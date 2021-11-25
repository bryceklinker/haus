export const DiagnosticsPage = {
    navigate: () => cy.navigate('Diagnostics'),
    findHeader: () => cy.findByTestId('diagnostics-header')
}