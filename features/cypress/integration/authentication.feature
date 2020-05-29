Feature: HAUS Authentication

  Scenario: Successful User Authentication
    Given I have valid user credentials
    When I login to HAUS
    Then I should see the HAUS dashboard