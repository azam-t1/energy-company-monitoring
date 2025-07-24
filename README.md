# Energy Company Monitoring

## User Story
As an Energy Company Account Manager, I want to be able to load a CSV file of Customer Meter Readings So that we can monitor their energy consumption and charge them accordingly

## Task
Ensuring the Acceptance Criteria are met, build a C# Web API that connects to an instance of a database and persists the contents of the Meter Reading CSV file.

We have provided you with a list of test customers along with their respective Account IDs (please refer to Test_Accounts.csv). Please seed the Test_Accounts.csv data into your chosen data storage technology and validate the Meter Read data against the accounts.

## Acceptance Criteria

### MUST HAVE

1) Create the following endpoint:
	
	POST => /meter-reading-uploads

2) The endpoint should be able to process a CSV of meter readings. An example CSV file has been provided (Meter_reading.csv)

3) Each entry in the CSV should be validated and if valid, stored in a DB.

4) After processing, the number of successful/failed readings should be returned.

**Validation:** 
- You should not be able to load the same entry twice
- A meter reading must be associated with an Account ID to be deemed valid
- Reading values should be in the format NNNNN

### NICE TO HAVE
1) Create a client in the technology of your choosing to consume the API. You can use angular/react/whatever you like

2) When an account has an existing read, ensure the new read isn't older than the existing read

3) Tests

4) CI/CD pipeline in GitHub
