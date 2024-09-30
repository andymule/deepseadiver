import pandas as pd
import numpy as np
import json

# Load the CSV file
data = pd.read_csv("DataSampleOctoTimespan.csv")

# Clean the dataframe
data.columns = [
    "timestamp",
    "Salinity_psu",
    "SoundVelocity_meters_per_second",
    "Temperature_celsius",  # Corrected 'celcius' typo
    "VehicleAltitude_meters",
    "VehicleDepth_meters",
    "VehicleHeading_degrees",
    "VehicleLatitude_degrees",
    "VehicleLongitude_degrees",  # Corrected typo
]

# Remove header row if duplicated
if data.iloc[0]["timestamp"] == "timestamp":
    data = data.drop(0).reset_index(drop=True)

# Convert columns to appropriate types
numeric_columns = [
    "Salinity_psu",
    "SoundVelocity_meters_per_second",
    "Temperature_celsius",
    "VehicleAltitude_meters",
    "VehicleDepth_meters",
    "VehicleHeading_degrees",
    "VehicleLatitude_degrees",
    "VehicleLongitude_degrees",
]
for col in numeric_columns:
    data[col] = pd.to_numeric(data[col])

# Convert timestamp to datetime
data["timestamp"] = pd.to_datetime(data["timestamp"])

# Prepare data for visualization
latitudes = data["VehicleLatitude_degrees"]
longitudes = data["VehicleLongitude_degrees"]
depths = data["VehicleDepth_meters"]
altitudes = data["VehicleAltitude_meters"]
timestamps = data["timestamp"]

# Reference positions (starting point)
lat_ref = latitudes.iloc[0]
lon_ref = longitudes.iloc[0]

# Accurate constants
meters_per_degree_lat = 111132.92  # meters per degree latitude
meters_per_degree_lon = 111319.5   # meters per degree longitude at the equator

# Convert latitude and longitude to meters relative to starting point
latitude_meters = (latitudes - lat_ref) * meters_per_degree_lat
longitude_meters = (longitudes - lon_ref) * meters_per_degree_lon * np.cos(np.radians(lat_ref))

# Create a new dataframe with the converted positions and other relevant data
converted_data = pd.DataFrame({
    'timestamp': data['timestamp'].astype(str),  # Convert timestamps to strings for JSON serialization
    'Salinity_psu': data['Salinity_psu'],
    'SoundVelocity_meters_per_second': data['SoundVelocity_meters_per_second'],
    'Temperature_celsius': data['Temperature_celsius'],
    'VehicleAltitude_meters': data['VehicleAltitude_meters'],
    'VehicleDepth_meters': data['VehicleDepth_meters'],
    'VehicleHeading_degrees': data['VehicleHeading_degrees'],
    'x_meters': longitude_meters,
    'y_meters': -depths,  # Depths as negative to go down in Unity's coordinate system
    'z_meters': latitude_meters,
})

# Adjust positions so that the first point is at (0, 0, 0)
x0 = converted_data['x_meters'].iloc[0]
y0 = converted_data['y_meters'].iloc[0]
z0 = converted_data['z_meters'].iloc[0]

converted_data['x_meters'] = converted_data['x_meters'] - x0
converted_data['y_meters'] = converted_data['y_meters'] - y0
converted_data['z_meters'] = converted_data['z_meters'] - z0

# Convert dataframe to a list of dictionaries
data_list = converted_data.to_dict(orient='records')

# Wrap the data in a dictionary under the key 'data' for Unity's JsonUtility
json_data = {"data": data_list}

# Export to JSON
with open('DataSampleOctoTimespan_converted.json', 'w') as json_file:
    json.dump(json_data, json_file)
