# GLVC

Tool to check if a Geometry Dash level has only features of a given version.

## Manual

1. Download the latest version in [GLVC](https://github.com/lucastozo/GLVC/releases), extract and run `GLVC.exe`
2. Insert a `.gmd` file path (for example: `"C:\path\to\your\folder\level.gmd"`)
3. Pick a GD version and check if the level is possible or not

## Example

In this example i will be checking if the level The Hell Zone by Sohn0924 is possible in Geometry Dash 1.2:

I specified where the level file is located, in my case, i exported the `.gmd` file of The Hell Zone in `C:\Users\Tozo\Desktop\levels`

And after specifying the file path, just pick a Geometry Dash version to check the level, in my case, i will be choosing `5 - 1.2`

![Screenshot_1](https://github.com/lucastozo/GLVC/assets/102305949/9b363e9f-e5d6-4239-9eaa-98fb8715c621)

In my example, The Hell Zone by Sohn0924 returned `Success: Possible Level` and the program waits any input to restart.

## Error Messages

* `Error: Illegal song ID: songID` or `Error: Illegal custom song ID: songID` - Level uses an song not present in the version or the level uses a custom newgrounds song, respectively.
* `Error: Illegal object ID: objectID, Position X,Y: posX, posY` - Level uses a possible song but has an illegal object for the specified version.

If you got the `Illegal object ID` error, the program will ask if you want to generate a .gmd file without illegal objects, if yes, the file will be generated in the folder `levels` inside the program folder (the folder containing `GLVC.exe`)
