# Editing xsd files and generating cs files

# In my experience the easiest way to edit the xsd files is with a text editor outside VS.
# A previous version of VS would do the code generation automatically but that changed with
# a version update - editing the xsd in VS resulted in bad things. Once edited, use xsd.exe
# from the VS install to generate the cs file from the xsd. In practice I do this on a copy
# of the committed xsd, generate the cs in a temporary directory and then paste the changes
# into the committed files.
#
# The one time so far I've had to add a new one I ended up adding a new xsd file and hacking
# the .csproj file to add the corresponding .cs file to the project as the .cs wasn't picked
# up by default.


# VS2010?
"c:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" Config.xsd /c /l:cs /n:OPPF.XML /o:C:\Formulatrix\OPPFImagerLink\OPPF\XML
"c:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" ImageInfo.xsd /c /l:cs /n:OPPF.XML /o:C:\Formulatrix\OPPFImagerLink\OPPF\XML

# VS2013 equivalent
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" Config.xsd /c /l:cs /n:OPPF.XML /o:D:\FormulatrixFromRepo\Tmp\OPPF\XML
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" ImageInfo.xsd /c /l:cs /n:OPPF.XML /o:D:\FormulatrixFromRepo\Tmp\OPPF\XML
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" PlateTypes.xsd /c /l:cs /n:OPPF.XML /o:D:\FormulatrixFromRepo\Tmp\OPPF\XML
