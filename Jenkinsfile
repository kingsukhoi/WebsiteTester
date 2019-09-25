pipeline {
  agent any
  stages {
    stage('publish') {
      steps {
        sh 'dotnet publish -r win-x64 --self-contained -c Release'
      }
    }
  }
}