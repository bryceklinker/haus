import {AUTH_STATE, ensureUserIsAuthenticated} from "./auth-handler";

describe('auth-handler', () => {
    describe('ensureUserIsAuthenticated', () => {
        let userManager;

        beforeEach(() => {
            userManager = {
                getUser: jest.fn(),
                signinRedirect: jest.fn(),
                signinRedirectCallback: jest.fn(),
            };
        })

        test('when user has not logged in then redirects user to signin', async () => {
            userManager.getUser.mockResolvedValue(null);
            userManager.signinRedirect.mockResolvedValue(null);

            const result = await ensureUserIsAuthenticated(userManager, 'code');

            expect(result).toEqual({state: AUTH_STATE.UNAUTHENTICATED, user: null});
            expect(userManager.signinRedirect).toHaveBeenCalled();
        })

        test('when user is logged in then returns user', async () => {
            userManager.getUser.mockResolvedValue({expired: false});

            const result = await ensureUserIsAuthenticated(userManager, 'code');
            expect(result).toEqual({state: AUTH_STATE.AUTHENTICATED, user: {expired: false}});
        })
        
        test('when user is returning with expired token then user is redirected', async () => {
            userManager.getUser.mockResolvedValue({expired: true});

            const result = await ensureUserIsAuthenticated(userManager, 'code');

            expect(userManager.signinRedirect).toHaveBeenCalled();
            expect(result).toEqual({state: AUTH_STATE.UNAUTHENTICATED, user: null});
        })
        
        test('when redirected with error then returns error state', async () => {
            window.location.query = '?error=4234234';
            
            const result = await ensureUserIsAuthenticated(userManager, 'code');
            
            expect(result).toEqual({state: AUTH_STATE.ERROR, user: null});
        });
        
        test('when redirected with response type then returns user from signin', async () => {
            userManager.signinRedirectCallback.mockResolvedValue({expired: false})
            window.location.query = '?code=1234324';
            
            const result = await ensureUserIsAuthenticated(userManager, 'code');
            
            expect(result).toEqual({state: AUTH_STATE.AUTHENTICATED, user: {expired: false}});
        })
    })
})