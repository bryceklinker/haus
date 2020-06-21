Feature: Manage Users
  
  Scenario: Create User
    When I create a user
    Then I should see user in list of users