# GeneratorDataProcessor
## Overview
- This .NET 8 console app **processes all existing `.xml` files** in the configured input folder **on startup**.
- Then it **watches** the same folder for **new** `.xml` files, processing them as they appear.
- Calculation logic (Totals, Max Daily Emissions, Heat Rates) is **in the code** (SOLID & Strategy Pattern).
- **Outputs** results as `*-Result.xml` in the configured output folder.

## Configuration
 **appsettings.json** (placed in the same folder as your `.csproj`):
   ```json
   {
     "Settings": {
       "InputFolderPath": ".",
       "OutputFolderPath": ".",
       "ReferenceDataFilePath": "ReferenceData.xml"
     }
   }
   ```
   Adjust paths as needed.
