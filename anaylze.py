import pandas as pd
import plotly.graph_objects as go
import numpy as np
from datetime import datetime

# Load the CSV file
file_path = "DataSampleOctoTimespan.csv"
data = pd.read_csv(file_path)

# Clean the dataframe by assigning correct column names and converting types
data.columns = [
    "timestamp",
    "Salinity_psu",
    "SoundVelocity_meters_per_second",
    "Temperature_celcius",
    "VehicleAltitude_meters",
    "VehicleDepth_meters",
    "VehicleHeading_degrees",
    "VehicleLatitude_degrees",
    "VehicleLongitude_degrees",
]

# Drop rows with missing or non-numeric values
cleaned_data = data.drop(0).copy()  # The first row contains column names

# Convert columns to appropriate types
cleaned_data["VehicleHeading_degrees"] = pd.to_numeric(
    cleaned_data["VehicleHeading_degrees"]
)
cleaned_data["VehicleLatitude_degrees"] = pd.to_numeric(
    cleaned_data["VehicleLatitude_degrees"]
)
cleaned_data["VehicleLongitude_degrees"] = pd.to_numeric(
    cleaned_data["VehicleLongitude_degrees"]
)
cleaned_data["VehicleDepth_meters"] = pd.to_numeric(cleaned_data["VehicleDepth_meters"])
cleaned_data["VehicleAltitude_meters"] = pd.to_numeric(
    cleaned_data["VehicleAltitude_meters"]
)

# Convert timestamp to datetime object for hover and color progression
cleaned_data["timestamp"] = pd.to_datetime(cleaned_data["timestamp"])

# Prepare data for visualization
latitudes = cleaned_data["VehicleLatitude_degrees"]
longitudes = cleaned_data["VehicleLongitude_degrees"]
depths = cleaned_data["VehicleDepth_meters"]
altitudes = cleaned_data["VehicleAltitude_meters"]
timestamps = cleaned_data["timestamp"]
time_numeric = (
    timestamps - timestamps.min()
).dt.total_seconds()  # Convert time to numerical value for coloring

# Convert latitude and longitude to meters
latitude_meters = latitudes * 111139  # Convert latitude to meters (approx)
longitude_ref = longitudes.mean()  # Reference point for longitude
longitude_meters = (
    (longitudes - longitude_ref) * 111139 * np.cos(np.radians(latitudes.mean()))
)  # Longitude to meters

# Scaling factor for zoom adjustment
zoom_factor = 10  # Keep the zoom factor the same

# Create a 3D scatter plot of the vehicle's path over time
fig = go.Figure()

# Scatter plot for the vehicle's position, colored by time progression
fig.add_trace(
    go.Scatter3d(
        x=latitude_meters,
        y=longitude_meters,
        z=depths,  # Depth as the z-axis
        mode="markers+lines",  # Use 'lines' to connect dots to show the path
        marker=dict(
            size=5,
            color=time_numeric,
            colorscale="Viridis",
            colorbar=dict(title="Time Progression (s)"),
        ),
        line=dict(color="blue", width=2),
        text=[f"Timestamp: {t}" for t in timestamps],  # Add timestamp for hover info
        hoverinfo="text",  # Show timestamp on hover
        name="Vehicle Path",
    )
)

# Adding a brown line to represent VehicleAltitude_meters
for i in range(len(latitude_meters)):
    fig.add_trace(
        go.Scatter3d(
            x=[latitude_meters.iloc[i], latitude_meters.iloc[i]],  # Constant latitude
            y=[
                longitude_meters.iloc[i],
                longitude_meters.iloc[i],
            ],  # Constant longitude
            z=[
                depths.iloc[i],
                depths.iloc[i] - altitudes.iloc[i],
            ],  # Altitude line below the depth
            mode="lines",
            line=dict(color="brown", width=2),  # Brown line
            name="Vehicle Altitude",
        )
    )

# Adjust axis ranges based on the zoom factor
fig.update_layout(
    title="3D Vehicle Path with Time Progression and Altitude (Meters)",
    scene=dict(
        xaxis_title="Latitude (meters)",
        yaxis_title="Longitude (meters)",
        zaxis_title="Depth (meters)",  # Updated z-axis to represent depth
        xaxis=dict(
            range=[
                latitude_meters.min()
                - (latitude_meters.max() - latitude_meters.min()) / zoom_factor,
                latitude_meters.max()
                + (latitude_meters.max() - latitude_meters.min()) / zoom_factor,
            ]
        ),  # Zoomed-in range for x-axis
        yaxis=dict(
            range=[
                longitude_meters.min()
                - (longitude_meters.max() - longitude_meters.min()) / zoom_factor,
                longitude_meters.max()
                + (longitude_meters.max() - longitude_meters.min()) / zoom_factor,
            ]
        ),  # Zoomed-in range for y-axis
        zaxis=dict(
            range=[depths.min() - 10, depths.max() + 10]
        ),  # Adjust range for depth
    ),
)

# Show the interactive 3D plot
fig.show()
