# Test assignment for Qoco

## Project Overview  
This tool automates manual Excel‑based quality checks, improving efficiency for aviation operations teams.

This solution has a target framework of .NET 8. The API implements the following: 
- Reads the flights.csv file provided to src/Data directory on the server
- Converts the CSV data into a structured JSON format and makes it available at GET api/flights
- Analyzes flight sequences to detect inconsistencies (mismatched departure/arrival airports). Full list of inconsistencies in JSON format is available at GET api/flights/inconsistencies. 

---

## Features
- **CSV Parsing**: Utilizes CsvHelper package to load and map CSV data.  
- **Swagger UI**: Interactive API documentation in development mode. Navigate to http://localhost:5000/swagger in your browser.
- **Unit Tests**: xUnit tests to validate CSV parsing and sequence‑analysis logic.
---

## Repository Structure
```
Qoco-Airlines/
├── .gitignore
├── Qoco-Airlines.sln
├── Run.bat
├── README.md
├── src/
│   ├── Controllers/
│   │   └── FlightsController.cs
│   ├── Data/
│   │   └── flights.csv
│   ├── Models/
│   │   ├── Flight.cs
│   │   ├── FlightMap.cs
│   │   └── Inconsistency.cs
│   ├── Services/
│   │   ├── FlightService.cs
│   │   └── IFlightService.cs
│   ├── Program.cs
│   └── Qoco-Airlines.csproj
└── Tests/
    ├── FlightServiceTests.cs
    └── Qoco-Airlines-Test.csproj
```

## Getting started
To execute the tests and start the app, execute `run.bat`. After tests run successfully and the solution builds, navigate to the API endpoints in your browser as defined below. The API is hosted on port 5000. 

## API Endpoints
| Method | URL                            | Description                               | Example command                                                                         |
| ------ | ------------------------------ | ----------------------------------------- |-----------------------------------------------------------------------------------------|
| GET    | `/api/flights`                 | Returns all flights as JSON.              | curl -X GET "http://localhost:5000/api/Flights" -H "accept: text/plain"                 |
| GET    | `/api/flights/inconsistencies` | Returns list of sequence inconsistencies. |	curl -X GET "http://localhost:5000/api/Flights/inconsistencies" -H "accept: text/plain" |

### Example Response for `/api/flights` 

```json
[
  {
    "id": 1,
    "registration": "ZX-IKD",
    "type": "350",
    "flightNumber": "M645",
    "departureAirport": "HEL",
    "departureTime": "2024-01-02T21:46:27",
    "arrivalAirport": "DXB",
    "arrivalTime": "2024-01-03T02:31:27"
  },
  {
    "id": 2,
    "registration": "ZW-TNZ",
    "type": "787",
    "flightNumber": "K319",
    "departureAirport": "SFO",
    "departureTime": "2023-03-25T15:55:27",
    "arrivalAirport": "DXB",
    "arrivalTime": "2023-03-25T17:53:27"
  },
  ...
]
```

### Example Response for `/api/flight/inconsistencies` 

```json
[
  {
    "flight": {
      "id": 997,
      "registration": "G-DIX",
      "type": "320",
      "flightNumber": "AY120",
      "departureAirport": "HEL",
      "departureTime": "2024-01-30T17:00:00",
      "arrivalAirport": "OUL",
      "arrivalTime": "2024-01-30T18:30:00"
    },
    "expectedNextDeparture": "OUL"
  }
  ...
]
```
