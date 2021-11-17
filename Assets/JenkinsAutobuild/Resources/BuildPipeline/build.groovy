

pipeline{

    agent any
    environment {
        GIT_URL = '%GIT_URL%'
        UNITY_VERSION="%UNITY_VERSION%"
        PROJECT_NAME = "%PROJECT_NAME%"
        KEY_NAME = "%KEY_NAME%.keystore"
        ALIAS = "%ALIAS%"
        ALIAS_PASS = "%ALIAS_PASS%"
        KEY_PASS = "%KEY_PASS%"

        LOG_FILE = "/JenkinsLog/build.log"    
        BUILD_DIR ="JenkinsBuilds" 
        GH_TOKEN  = credentials('git-credential')
    }
    

    stages{    
      
        stage('Git Clone'){
            steps {
                //slackSend message: "${env.JOB_NAME} is started"
                git branch: '**', credentialsId: 'git-credential', url: "${GIT_URL}"
            }
        }
          stage('Semver'){
            steps{
                script{
                    sh "chmod +x semver-gen"
                    env.APP_VERSION=sh (script: "./semver-gen generate -l -s | sed 's/SEMVER//g'", returnStdout: true).trim()
                    echo """${APP_VERSION}"""
                }
            }
        }
         stage('Changelog'){
            steps{
                script{
                    
                    passedBuilds = []
                    lastSuccessfulBuild(passedBuilds, currentBuild);
                    env.CHANGELOG = getChangeLog(passedBuilds)
                    echo "${CHANGELOG}"
                }
            }
        }
          
        stage("Configure by commit message"){
            steps{
                 script {
                      env.AAB_BUILD = "false"
                      result = sh (script: "git log -1 | grep '.*\\[build aab\\].*'", returnStatus: true)
                      if (result == 0) {
                          env.AAB_BUILD = "true"
                          echo "'[build aab]' found in git commit message."
                      }else{
                          echo "Git commit not contains '[build aab]'"
                      }
                      
                      env.APK_BUILD = "false"
                      result = sh (script: "git log -1 | grep '.*\\[build apk\\].*'", returnStatus: true)
                      if (result == 0) {
                          env.APK_BUILD = "true"
                          echo "'[build apk]' found in git commit message."
                      }else{
                          echo "Git commit not contains '[build apk]'"
                      }
                 }
            }
        }

        stage('Build Step'){
            steps {            
               script {
                env.WORKING_DIRECTORY=sh(
                    script: "pwd",
                    returnStdout: true
                ).trim()
                echo "${WORKING_DIRECTORY}"
                string BUILD_PATH="${WORKING_DIRECTORY}/${BUILD_DIR}/"
                string KEYSTORE_PATH="${WORKING_DIRECTORY}/Keystore/${KEY_NAME}"
                string FILE_NAME="${PROJECT_NAME}:${APP_VERSION}"
                sh '''cd Assets && nuget restore && cd ..'''
                
                }
                script{
                    if(env.APK_BUILD=="true"){
                       echo "[LOG] Started APK build"
                       sh """
                        cd ${BUILD_DIR}
                        rm -rf *.apk
                        echo "removed all old apks"
                        cd ..
                        /Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity -batchmode -nographics -silent-crashes -logFile "${WORKING_DIRECTORY}${LOG_FILE}" -quit -projectPath ${WORKING_DIRECTORY} -buildTarget Android -executeMethod AutoBuild.BuildViaPipeLine --args filename ${FILE_NAME} version ${APP_VERSION} keypath ${KEYSTORE_PATH} buildpath ${BUILD_PATH} keyalias ${ALIAS} keypass ${KEY_PASS} aliaspass ${ALIAS_PASS}

                        """
                    }
                    
                    if(env.AAB_BUILD=="true"){
                       echo "[LOG] Started AAB build"
                       sh """
                       cd ${BUILD_DIR}
                       rm -rf *.aab
                       echo "removed all old aabs"
                       cd ..
                        /Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity -batchmode -nographics -silent-crashes -logFile "${WORKING_DIRECTORY}${LOG_FILE}" -quit -projectPath ${WORKING_DIRECTORY} -buildTarget Android -executeMethod AutoBuild.BuildAABViaPipeLine --args filename "${FILE_NAME}" version ${APP_VERSION} keypath ${KEYSTORE_PATH} buildpath ${BUILD_PATH} keyalias ${ALIAS} keypass ${KEY_PASS} aliaspass ${ALIAS_PASS}
                        """
                    }
                    if(env.APK_BUILD=="false" && env.AAB_BUILD=="false"){
                         currentBuild.result = 'NOT_BUILT' 
                    }
                }              
            }

        }


    stage('Slack Notification'){
                steps {
                    script {            
                        if(env.AAB_BUILD=="true"){
                        echo "Start upload aab"
                        androidApkUpload googleCredentialsId: 'GooglePlayCredential',
                                                         filesPattern: '**/*.aab',
                                                         trackName: 'internal',
                                                         rolloutPercentage: '100',
                                                         releaseName: """${APP_VERSION}""",
                                                         //deobfuscationFilesPattern: """**/*${APP_VERSION}_mapping.txt""",
                                                         //nativeDebugSymbolFilesPattern: '**/build/outputs/**/native-debug-symbols.zip',
                                                         inAppUpdatePriority: '2',
                                                         recentChangeList: [
                                                           [language: 'en-GB', text: "Uploaded Jenkins build."]                           
                                                         ]
                        slackSend color: "good", message: "${PROJECT_NAME}:${APP_VERSION}: AAB build completed\n "+ "${CHANGELOG}"
                        }
                        if(env.APK_BUILD=="true"){
                        slackSend color: "good", message: "Uploading Build"
                        slackUploadFile filePath: "**/*.apk", initialComment:  "Uploading Completed\n "+ "${CHANGELOG}"
                        }
                    }
                   
                }
            }
    }
}

def lastSuccessfulBuild(passedBuilds, build) {
  if ((build != null) && (build.result != 'SUCCESS')) {
      passedBuilds.add(build)
      lastSuccessfulBuild(passedBuilds, build.getPreviousBuild())
   }
}

@NonCPS
def getChangeLog(passedBuilds) {
    def log = ""
    for (int x = 0; x < passedBuilds.size(); x++) {
        def currentBuild = passedBuilds[x];
        def changeLogSets = currentBuild.rawBuild.changeSets
        for (int i = 0; i < changeLogSets.size(); i++) {
            def entries = changeLogSets[i].items
            for (int j = 0; j < entries.length; j++) {
                def entry = entries[j]
                log += "${entry.msg} by ${entry.author} \n"
            }
        }
    }
    return log;
  }

