pipeline {
    agent any
    environment {
        YARN_VERSION = '1.22.4'
    }

    stages {
        stage('Run Tests') {
            steps {
                sh './scripts/run_unit_tests.sh'
                sh './scripts/run_acceptance_tests.sh'
            }
        }
    }
}