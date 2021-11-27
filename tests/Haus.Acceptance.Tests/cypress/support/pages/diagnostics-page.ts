export const DiagnosticsPage = {
    navigate: () => cy.navigate('Diagnostics'),
    findHeader: () => cy.findByLabelText('diagnostics header')
}