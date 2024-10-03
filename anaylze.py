import pandas as pd
import plotly.graph_objects as go
import numpy as np

# Load the CSV file
data = pd.read_csv("DataSampleOctoTimespan.csv")

# Clean the dataframe
data.columns = [
    "timestamp",
    "Salinity_psu",
    "SoundVelocity_meters_per_second",
    "Temperature_celcius",
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
    "VehicleHeading_degrees",
    "VehicleLatitude_degrees",
    "VehicleLongitude_degrees",
    "VehicleDepth_meters",
    "VehicleAltitude_meters",
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
time_numeric = (timestamps - timestamps.min()).dt.total_seconds()

# Reference positions (starting point)
lat_ref = latitudes.iloc[0]
lon_ref = longitudes.iloc[0]

# Accurate constants
DEGREE = 111319.5  # meters per degree at the equator

# Convert latitude and longitude to meters relative to starting point
latitude_meters = (latitudes - lat_ref) * 111132.92
longitude_meters = (longitudes - lon_ref) * DEGREE * np.cos(np.radians(latitudes))

# Scaling factor for zoom adjustment
zoom_factor = 1  # Adjusted for relative movement

# Create a 3D scatter plot of the vehicle's path over time
fig = go.Figure()

# Scatter plot for the vehicle's position, colored by time progression
fig.add_trace(
    go.Scatter3d(
        x=latitude_meters,
        y=longitude_meters,
        z=depths,
        mode="markers+lines",
        marker=dict(
            size=5,
            color=time_numeric,
            colorscale="Viridis",
            colorbar=dict(title="Time Progression (s)"),
        ),
        line=dict(color="blue", width=2),
        text=[f"Timestamp: {t}" for t in timestamps],
        hoverinfo="text",
        name="Vehicle Path",
    )
)

# Adding a brown line to represent VehicleAltitude_meters extending upward
for i in range(len(latitude_meters)):
    fig.add_trace(
        go.Scatter3d(
            x=[latitude_meters.iloc[i], latitude_meters.iloc[i]],
            y=[longitude_meters.iloc[i], longitude_meters.iloc[i]],
            z=[depths.iloc[i], depths.iloc[i] + altitudes.iloc[i]],  # Add altitude to depth for upward extension
            mode="lines",
            line=dict(color="brown", width=2),
            showlegend=False,  # Avoid duplicate legend entries
            hoverinfo="none",
        )
    )

# Adjust axis ranges based on the zoom factor and flip depth axis
fig.update_layout(
    title="3D Vehicle Path with Time Progression and Altitude (Meters from Start)",
    scene=dict(
        xaxis_title="Latitude (meters from start)",
        yaxis_title="Longitude (meters from start)",
        zaxis_title="Depth (meters)",
        xaxis=dict(
            range=[
                latitude_meters.min() - 5,
                latitude_meters.max() + 5,
            ]
        ),
        yaxis=dict(
            range=[
                longitude_meters.min() - 5,
                longitude_meters.max() + 5,
            ]
        ),
        zaxis=dict(
            range=[depths.max() + 10, depths.min() - 10],  # Flip depth axis
            autorange="reversed"  # Ensure axis is reversed to show correct orientation
        ),
    ),
)

# Show the interactive 3D plot
fig.show()
