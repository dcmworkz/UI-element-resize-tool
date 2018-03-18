# UI-element-resize-utility
=================================

Lairinus Element Resizer
    - Developed by Dillon Mosono (lairinus@gmail.com, dillonmosono@gmail.com, Lairinus, Shingox, @Lairinus)
	- http://lairinus.com

=================================

Introduction:
I've been coming out with a lot of tools for Unity3D recently, and one thing that was annoying for me is coming up with a decent UI. There was careful planning paired with the creation of custom UI graphics, and then the most annoying part of it - getting everything to be the right size.

UI elements in Unity3D can be tricky and time-consuming to resize. Also, it involves a little bit of math if you want to make it percent based. For instance, let's say you have a background image that you want to take up 50% of the screen height and 75% of its' width. You could drag the Image element until it looks right, or you could be more-precise and enter a width and height pixels. In Unity, there is no way to do this without a calculator or mental math.

The UI Element Resizer window takes care of this issue for you. You can specify changing an element's width, height, and whether or not to use pixels or percentages.

Features:
Can use both Pixels or Percentages when resizing elements
Allows resizing of multiple objects at once
With percentages, element's can be resized based off of their parents or themselves.
A separate, custom window can be docked and made at-the-ready for a full session of resizing.
Allows centering and maintaining an element's old position
Ease of use:
In order to use this component, import it into the Project's "Editor" asset folder. After that, do the following:

Navigate to "Lairinus UI -> Windows -> Resizer tool"
Select as many UI Elements as you want
Set the values in the UI Resizer window
Click "Resize"
All of the elements that you selected (that have RectTransform components) will be resized!

=================================

Current Version: 1.0.0

Changelog:

Version 1.0.0 - Initial Release!
