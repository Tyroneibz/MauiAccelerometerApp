using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using System;

namespace MauiAccelerometerApp
{
    public partial class MainPage : ContentPage
    {
        private double velocityX = 0;
        private double velocityY = 0;
        private double velocityZ = 0;
        private DateTime lastUpdate;
        private const double DampingFactor = 0.98;  // Realistischer Dämpfungsfaktor

        public MainPage()
        {
            InitializeComponent();
            Accelerometer.Start(SensorSpeed.UI);
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            lastUpdate = DateTime.Now;
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            DateTime currentTime = DateTime.Now;
            double deltaTime = (currentTime - lastUpdate).TotalSeconds;
            lastUpdate = currentTime;

            // Berechnung der Geschwindigkeit basierend auf der Beschleunigung
            velocityX += data.Acceleration.X * deltaTime;
            velocityY += data.Acceleration.Y * deltaTime;
            velocityZ += data.Acceleration.Z * deltaTime;

            // Anwendung der Dämpfung
            velocityX *= DampingFactor;
            velocityY *= DampingFactor;
            velocityZ *= DampingFactor;

            // Berechnung der Gesamtgeschwindigkeit
            double totalSpeed = Math.Sqrt(velocityX * velocityX + velocityY * velocityY + velocityZ * velocityZ);

            // Beispiel: Anzeigen der X-, Y-, und Z-Beschleunigungswerte und Geschwindigkeit in Labels
            Device.BeginInvokeOnMainThread(() =>
            {
                XLabel.Text = $"X: {FormatValue(data.Acceleration.X)}";
                YLabel.Text = $"Y: {FormatValue(data.Acceleration.Y)}";
                ZLabel.Text = $"Z: {FormatValue(data.Acceleration.Z)}";
                SpeedLabel.Text = $"You are moving at {FormatValue(totalSpeed)} m/s";
            });

            // Debug-Ausgabe
            System.Diagnostics.Debug.WriteLine($"X: {data.Acceleration.X}, Y: {data.Acceleration.Y}, Z: {data.Acceleration.Z}, Total Speed: {totalSpeed}");
        }

        private string FormatValue(double value)
        {
            // Hier runden wir den Wert auf 2 Dezimalstellen und vermeiden -0.0
            value = Math.Round(value, 2);
            return value == 0 ? "0.00" : value.ToString("F2");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Accelerometer.Stop();
        }
    }
}
