Copy240x320Viewsto320x240.exe (Run manually via bat file)
=============================
This program will copy all files from the 240x320 to the 320x240 project and preserve the visual layout.
Also does search and replace for the following
It searches and replaces namespace to be 320x240
Looks for "preserved words" Size,Location,Text,Font and keeps these the same as what they were in the 320x240 project. This allows the copy to not change the visual layout.
Saves the files.

To Use:
Checkout entire 320x240 project except for:
- frmMain.cs
- frmDialogContainer.cs
- frmErrorReport.cs
Those files need to manually be updated.

Edit sample_Copy320x240Files.bat "programLocation" to point to the program
Save the bat somewhere on your local computer.
Run the bat file.

Remarks:
If there are some new files that were created for the 320x240 view you have to use visual studio to manually include those into the project.
You should also manually edit any new files to adjust the size of control for the widescreen.
Show all files > Right click file > Include in project.

1 file will be created
newfileslist.txt - list of all the new files added to the 320x240 project

When done, you will need to merge or update frmMain.cs and frmDialog.cs if there are any changes there.

ConvertFormToMultilanguage.exe (Automatically run as part of dMobile.Bedrock prebuild event)
==============================
Requires build configuration Multilanguage to be selected (next to the start debugging button).

Each project build configuration Multilanguage has a variable added to it to determine that the Multilanguage build is selected.
See Project properties > Build (Configuration: Multilanguage)  > General > Conditional Compilation Symbols: MULTILANGUAGE
Searches through all forms in a directory, looking for text to translate.
Uses #if MULTILANGUAGE #else #endif preprocessor blocks to conditionally insert the multilanguage code.
This enables multilanguage code to not seep into regular non-multilanguage builds.

Checks designer files for string literals and adds a translation to the class after InitializeComponents().
Checks form class files for string literals and creates an #if/#else/#endif block around it. This works automatically for one literal per line.
If more than one string literal exists on a line, it will need to be added via another script (see sample_ScanMultilanguage.bat)

Updates english translation file


To Use:
Checkout dMobile.Bedrock/Language/dMobileTranslation.ENU.txt	(Allows automatic English translation population key=value where key is the enumeration and value is the text)
Checkout dMobile.Bedrock/Language/dMobileTranslation.FRC.txt	(Allows automatic French translation population key=value where key is the enumeration and value is the text)
Checkout dMobile.Bedrock/TranslatorVocabulary.tt				(Updates count of the number of enumerated values)
Checkout dMobile.Bedrock/GeneratedTranslator.tt					(New vocabulary gets added as an enumeration here)
Checkout entire project 240x320									(Files to be scanned and updated/modified)
Select Configuration: Multilanguage								(Use multilanguage code and run prebuild events for multilanguage)
Build dMobile.Bedrock project									(Scans 240x320 for code, inserts, updates, adds translations, adds enumerations, automates translations)
Build 240x320 project											(Normal build with updated files)

ConvertFormToMultilanguage.exe (Run via bat file. Manual adding harder to guess/automate string literals)
==============================
Not every string can be automated because some are SQL, XML, evaluative, etc.
This feature fills the gap by asking a user to say yes or no to each string.
Automates adding #if/#else/#endif blocks, adding word to english translation (which will then be automatically translated by automatic version of this executable).

To Use:															(Allows you to create translations where there is more than one string literal in a line)
Checkout entire solution										(We are going to be updating almost everything)
Copy sample_ScanMultilanguage.bat to local computer
Edit sample_ScanMultilanguage.bat projexe= to location of ConvertFormToMultilanguage.exe
Save bat
Run bat
This now launches a GUI where the user is asked to evaluate lines of text.

Controls:
Y = yes, this should be translated.
N = no, this should not be translated.
Esc = skip file
S = save file (will also be prompted at the end)
Left/Right Arrow keys - go back/skip forward
M = mark a file (makes a one line of text saying you should go back and review this file)

2 files will be created
Cache.txt - list of files that have been scanned. Delete a file or all files from here to allow the program to scan it again.
marks.txt - list of marked files. Delete at will or review to see what you need to see in code again.

Translator.exe (Automatic - part of bedrock prebuild event)
==============
Scans english and french translation files (key=value)
If french has a matching key in english, it is skipped.
If french is missing a key, the english text is run through Microsoft Azure translator and added to the french file
If french has an extra key that english does not have, it is removed

To Use:
Checkout dMobile.Bedrock/Language/dMobileTranslation.FRC.txt (Or whatever language you will be updating. Just add prebuild script for it.)
Build dMobile.Bedrock project

Remarks:
Have the output window selected with Show output from Build to see progress
Automatic translations take some time to run. Only missing translations will be translated.
If you have a lot of new translations, it can take hours (~1000+ translations taking several hours). But for the most part it will take less than a minute.


ExcelWriter.exe (Manual via bat file
===============
Exports multiple translations to a single excel file.
Imports a translation from an excel file.
Performs fuzzy logic
Removes lines based off an exclusion list
Merges two token files


Example Useage:
sample_ExportLanguagesToExcel.bat				(Writes language files to excel)
sample_ImportLanguagesFromExcel.bat				(Writes language file from excel)
sample_ProcessTranslationsAgainstdMobile.bat	(Imports an excel translation to a token file,
												 merges dMobiles two language files english and french,
												 looks for matches in translated file and the merged dMobile files,
												 removes properly translated lines from the dMobile translation files (temp file not original),
												 exports the cleaned dMobile translation files to excel - to give to translator)