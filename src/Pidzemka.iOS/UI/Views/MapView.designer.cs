// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Pidzemka.iOS.UI.Views
{
    [Register ("MapView")]
    partial class MapView
    {
        [Outlet]
        UIKit.UIImageView MapImageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MapImageView != null) {
                MapImageView.Dispose ();
                MapImageView = null;
            }
        }
    }
}