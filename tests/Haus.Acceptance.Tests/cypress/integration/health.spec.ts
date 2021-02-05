import {HealthPage} from "../support/pages";

describe('Health', () => {
    beforeEach(() => {
        HealthPage.navigate();
    })
    
    it('should show health dashboard', () => {
        HealthPage.getDashboard().should('be.visible');
        HealthPage.getRecentEvents().should('be.visible');
        HealthPage.getRecentLogs().should('be.visible');
    })
})