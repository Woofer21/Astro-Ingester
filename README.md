# AstroIngester Monorepo
This is the mono repo for my AstroIngester applications. The repo contains 3 projects:
1. AstroIngester CLI - This is the CLI application
2. AstroIngester UI - This is the UI version of the CLI application
3. AstroIngester Core - This is the core library, it contains the shared logic for the CLI and UI applications

## AstroIngester CLI
### Current Version ~ 0.1.1 Alpha

This is the CLI application. It can be used to move files from one directory to other directories.

### Usage
You are able to use the CLI in two ways:

#### 1. Command line arguments w/ config file (✅ Recommended)

Currently the CLI application has the following command line arguments:
1. `--Verbose`
    - Alt: `-v`
    - Description: Enables extra console logging
    - Usage: `astroingester --verbose`
2. `--Config`
    - Alt: `-c`
    - Description: The path to the config file
    - Arguments: 
        - `<path>`: The path to the config text file
    - Usage: `astroingester --config <path>`
    - To read more about the config file, please see [the config section](#config-file-arguments)

#### 2. Without command line arguments (❌ Not Recommended)
If you do not pass the CLI a config file, it will launch you into an interactive mode to configure your settings.

This option is <u>**Not Recommended**</u> due to it missing many of the features supported in the config file.

There is a plan to add these missing features in the future.

### Config File Arguments
The config file is a text file that contains a reuseable set of options for the CLI application.

You can denote comments in the config file by starting a line with a `#` or `//`. Blank lines are ignored.

Note: All the config options are case-insensitive, meaning `Verbose`, `verbose`, and `VeRbOsE` are all valid.

The config file accepts the following arguments:
1. `Verbose`
    - Description: Enables extra console logging, can only be used **once**
    - Usage:
    ```txt
    Verbose=<boolean>
    ```
    - Example:
    ```txt
    Verbose=true
    ```
2. `InputPath`
    - Alt: `Input_Path`
    - Description: The path to the directory you want to move files from, can only be used **once**
    - Usage:
    ```txt
    InputPath=<path>
    ```
    - Example:
    ```txt
    InputPath=G:/DCIM
    ```
4. `IgnoredInputPath`
    - Alt: `Ignored_Input_Path`
    - Description: These are paths to the directories which you do not want to move files from, supports `*` as a wild card. Ignored paths are recursive, meaning it will ignore all paths within the highest ignored path.
    - Usage:
    ```txt
    IgnoredInputPath=<path>
    ```
    - Example:
    ```txt
    IgnoredInputPath=D:/Photography/Plant Pictutes/
    # ^ This will ignore the plant pictures folder, and all folders within that folder
    
    IgnoredInputPath=D:/Photography/*Images/*/*
    # ^ This will ignore any folder that is 2 or more folders deep within any folder that ends with "Images",
    # eg: D:/Photography/Raw Images/June/Day 1/ will be ignored
    # eg: D:/Photography/Images/June/Day 1/ will be ignored
    # eg: D:/Photography/Images/June/ will NOT be ignored
    ```
5. `OutputSort`
    - Alt: `Output_Sort`
    - Description: The path to the directory where you want to move the files to, supports `<year>`, `<month>`, and `<day>` placeholders
    - Usage:
    ```txt
    OutputSort=<path>, [Extension[]], [Comment[]], [BeforeDate[]], [AfterDate[]], [Day[]], [Month[]], [Year[]]
    ```
    - Example:
    ```txt
    OutputSort=D:/Photography/<year>/<month>, Extension[.jpg], AfterDate[9/21/2025]
    ```
6. `OutputCopy`
    - Alt: `Output_Copy`
    - Description: Additional paths that you would like to copy files to, supports `<year>`, `<month>`, and `<day>` placeholders
    - Usage:
    ```txt
    OutputCopy=<path>, [Extension[]], [Comment[]], [BeforeDate[]], [AfterDate[]], [Day[]], [Month[]], [Year[]]
    ```
    - Example:
    ```txt
    OutputCopy=C:/Photography/Pre Processing/lights, Comment[L] Extension[.jpg], AfterDate[9/21/2025]
    ```

Some path arguments support the following placeholders:
1. `<year>`
    - Description: The year from the files `creationTime`
    - Usage:
    ```txt
    <year>
    ```
    - Example:
    ```txt
    ../Images/<year>
    ```
2. `<month>`
    - Description: The month from the files `creationTime`
    - Usage:
    ```txt
    <month>
    ```
    - Example:
    ```txt
    ../Images/<year>/<month>
    ```
3. `<day>`
    - Description: The day from the files `creationTime`
    - Usage:
    ```txt
    <day>
    ```
    - Example:
    ```txt
    ../Images/<year>/<month>/<day>
    ```

Some config options support the following filters, all filters are optional and act as an `AND` filter:
1. `Extension`
    - Description: The file extension(s) of the file, additional extensions are treated as `OR` filters
    - Usage:
    ```txt
    Extension[<extension>, [extension2], [extension3], ...]
    ```
    - Example:
    ```txt
    ... Extension[.jpg]
    ... Extension[.jpg, .png]
    ```
2. `Comment`
    - Description: Matches the input to the EXACT comment on the `comment` field in the file
    - Usage:
    ```txt
    Comment[<comment>]
    ```
    - Example:
    ```txt
    ... Comment[L]
    ... Comment[Orion Nebula]
    ```
3. `BeforeDate`
    - Alt: `Before_Date`
    - Description: Matches files that have a `creationTime` before the specified date, date format must be `Month/Day/Year`
    - Usage:
    ```txt
    BeforeDate[<date>]
    ```
    - Example:
    ```txt
    ... BeforeDate[9/21/2025]
    ```
4. `AfterDate`
    - Alt: `After_Date`
    - Description: Matches files that have a `creationTime` after the specified date, date format must be `Month/Day/Year`
    - Usage:
    ```txt
    AfterDate[<date>]
    ```
    - Example:
    ```txt
    ... AfterDate[9/21/2025]
    ```
5. `Day`
    - Description: Matches files that have a `creationTime` on the specified day integer, disregarding the month and year
    - Usage:
    ```txt
    Day[<day>]
    ```
    - Example:
    ```txt
    ... Day[21]
    ```
6. `Month`
    - Description: Matches files that have a `creationTime` on the specified month integer, disregarding the day and year
    - Usage:
    ```txt
    Month[<month>]
    ```
    - Example:
    ```txt
    ... Month[9]
    ```
7. `Year`
    - Description: Matches files that have a `creationTime` on the specified year integer, disregarding the month and day
    - Usage:
    ```txt
    Year[<year>]
    ```
    - Example:
    ```txt
    ... Year[2025]
    ```

### Example Config File
```txt
Verbose=true

InputPath=G:/DCIM

# Output paths for images
OutputSort=D:/Photography/Images/<year>/<month>, Extension[.jpg, .png]
OutputSort=F:/Photography/Images/<year>/<month>, Extension[.jpg, .png]

# Output paths for raws
OutputSort=D:/Photography/Raw Images/<year>/<month>, Extension[.nef]
OutputSort=F:/Photography/Raw Images/<year>/<month>, Extension[.nef]

# Output paths for files based on comments
OutputCopy=D:/Photography/Pre-Processing/Day 1/lights, Comment[L], Extension[.nef], AfterDate[9/27/2025]
OutputCopy=D:/Photography/Pre-Processing/Day 1/darks, Comment[D], Extension[.nef], AfterDate[9/27/2025]
OutputCopy=D:/Photography/Pre-Processing/Day 1/flats, Comment[F], Extension[.nef], AfterDate[9/27/2025]
OutputCopy=D:/Photography/Pre-Processing/Day 1/bias, Comment[B], Extension[.nef], AfterDate[9/27/2025]
```

## AstroIngester UI
### Current Version ~ N/A
Currently the UI application is just a placeholder, in the future it will be a UI version of the CLI application.

## AstroIngester Core
### Current Version ~ 0.1.0 Alpha
This is the core library for the shared logic between the CLI and UI applications, the code contained in this core library may change as I develop and see what is needed in only the CLI and only in the UI application.

Currently the core library contains the following logic:
1. (ConsoleHelpers.cs) The logic for console logging with colors, and "deleting" logged lines
2. (FileTools.cs) The main logic for holding the input directory, output directories, and their related configs
3. (MetadataTools.cs) The tool for getting a files comment and date
4. (MoveOperationItem.cs) The data structure that contains the config related to a files intended movement
5. (OutputPathItem.cs) The data structure that contains the config related to a folders filters