module.exports = {
    verbose: true,
    testPathIgnorePatterns: [
        '<rootDir>/build'
    ],
    setupFilesAfterEnv: [
        '<rootDir>/testing/jest.setup.js'
    ]
}
