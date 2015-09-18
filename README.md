# Font-Creator-For-Unity3D
This project is based on BMFont(www.angelcode.com/products/bmfont). BMFont works fine to create custom font from multiple images, but it's not so convinent when create a large and complicated font.
This project is trying to optimize BMFont's workflow in order to create font faster and easier. By loading a .csv configuration file which you can edit with excel or other text editors, you can easily create a font consists of images.

### usage
load .csv font configuration file

> start bin/bmfont.exe

> choose Options ==> Load font congfiuration...

> choose bin/sample.csv, bmfont will automatic load images descripted in this csv file

> choose Options ==> Save bitmap font as...

save .csv font configuration file

> start bin/bmfont.exe

> choose Options ==> Save font congfiuration..., bmfont will write images path into target file

configure file
> character : character you want to create
> image_path : absolute path of your custom image
> x_offset/y_offset/x_advance: bmfont original settings, just set to 0 if you don't understand what it means
> x/y/width/height : you should set these values if you want to use part of an image, if width/height is 0, bmfont will use original width/height


### changelog
- 09/18/2015 support use part of an image to create font
- 07/28/2015 version 1.0 released, can load and save .csv style font configuration file.

### bugs and issues

tomorrow.wakeup # gmail
