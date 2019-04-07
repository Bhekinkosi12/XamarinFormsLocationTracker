using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocationTracker;
using LocationTracker.Droid;
using  Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace LocationTracker.Droid
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap map;
        List<Position> routeCoordinates;
        CustomMap formsMap = null;
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;

                ((MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            base.OnMapReady(map);
            if (routeCoordinates.Count != 0)
            {
                map = googleMap;
                var polylineOptions = new PolylineOptions();
                polylineOptions.InvokeColor(0x66FF0000);
                polylineOptions.InvokeWidth(15f);

                foreach (var position in routeCoordinates)
                {
                    polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
                }
                map.AddPolyline(polylineOptions);
            }
        }
    }
}