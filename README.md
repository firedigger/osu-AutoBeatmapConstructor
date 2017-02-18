# osu-AutoBeatmapGenerator
Automated highly-customizable program for creating pattern-training difficulties from existing maps in osu!

After opening the program click File->Open .osu to import the base beatmap which will be used to initialize the timing, song and metadata. Most often this is going to be any existing map from your collection.  
  
Immediately the initial settings will be prompted. You can manually specify the begin and end (in ms of the timeline) the mapping period and you can also choose to preserve the untouched base map parts (remapping only kiai for example) or use the intuitive checkboxes to just use original map times.  
You can also manually set starting point for the map, which defaults to (200,200). The use of this might be to generate high spacing patterns which might be geometrically impossible from other positions (but that's next level tactics, don't worry).  
  
Now you can start add patterns with various settings by clicking Add context menu and selecting the desired pattern type.  
I'll try to give an instruction for the patterns and parameters:  
  
There are some universal parameters like "Number" = Number of specified patterns, and you can also choose "Till end" to tell the program to spam the patterns till the end of the mapping period.  
  
1) Polygons  
You can create Triangles, Squares, Pentagons ... up to 7 points.  
The checkbox "Star polygon order" will make the note order not traverse through the points of the polygon but make a cross-traversing, star-like pattern (for 5-polygon that will be exactly The star).  
Spacing is the size of the polygon, starting from nearly stacked to half-screen.  
Rotation is the angle for every sequential polygon to be turned, starting from 0 to 60 degrees.  
Shift is the shift for every sequential polygon, starting from 0 to some small value.  
  
2) Streams  
You can create streams!  
Number of points is the number of notes in a single generated streams with a consistent curve and direction. Next stream might have a jump, direction change, and has an NewCombo.  
Spacing is the distance between notes in the stream, from nearly 0 to REALLY spaced(most of the time you should be choosing left half of the slider).  
Curviness is the curviness of the stream, starting for linear to nearly circular, high values might drastically affect spacing.  
Shift is the shift for every sequential stream, starting from 0(in case of 0 it will default to the stream spacing itself) to some small value.  
  
WARNING! High values of spacing AND curviness might result in madness, and RARELY notes outside of the playfield, but you will get a warning if that's the case. Generating spaced curved streams is really hard honestly.  
  
3) Random stacks  
This was originally the tool to spam RANDOM jumps all over the place with consistent spacing but now you can also spam doubles and triples there.  
Number of notes is the number of notes.  
Spacing is the spacing, starting for barely anything upto nearly full-screen.  
Stacks lets you specify how many notes you want in a stack at MAX. For example, selecting 3 will RANDOMLY spam singles, double AND triples(so the max stack number + any small value).  
If you want specifically only triples you can check "Only such".  
  
4) 1-2 jumps  
You can create parallel pp jumps and circular 1-2 jumps!  
Jump type is one of the three. Main difference is in the default direction and shift direction. Horizontal jumps will be horizontal and shifted vertically, vertical jumps will be vertical and shifted horizontally and rotating jumps will be shifted in the direction perpendicular to every new 1-2 pair.  
Spacing is the spacing, starting for nearly 0 to half-screen.  
Rotation in case of horizontal and vertical jumps in the same static rotation angle applied to all pairs, relative to the axis of the pair. Defaulted to 0, it goes from -60 to 60 degrees.  
Shift is the shift for every sequential 1-2 jump, starting from 0 to some small value. If you want glorious circular jumps, you'll want to leave it 0.  
  
5) Breaks  
Sometimes after singletap or stream spam you'd want a break, and that exactly does it, for arbitrary number of seconds.  
  
This covers the patterns to be creates. The added patterns will be listed in the main window. You can also use Ctrl-C + Ctrl-V combination to copy patterns. You can delete one or multiple selected elements using the button or keyboard Del key (you can't move them though and you can only add a new pattern to the end of the list). You can also clear the list with Config-Clear list.  

To finally generate the beatmap, click Beatmap->Generate beatmap. That's the simplest forward pass, but there is some more stuff implemented so keep reading.  

As the pattern configuration is independent of the map itself, it can be stored separately and applied to any other map! You can create a full set of your own configuration in one file and use it to generate a full training mapset with several clicks!  
  
After finishing your configuration click Config->Save config to new file. The file formate is XML so that you can also look through the file in notepad and edit it yourself!
You can keep multiple configurations in one file by using Config->Save config to existing file. You can now load configs from the file in order to edit and save them, or to generate a new beatmap.  
  
Beatmap stats like CS,AR,OD,HP are also a part of the configuration, and can be specified at Beatmap->Override Beatmap stats. This enables you to create stuff like "High CS jump training" or various combinations of your same pattern configuration but with different CS,AR etc(You'll only need to do the long process of creating the configurations once as they can be saved!). I think that should be handy.  
  
The last but not least feature is generating a random config. Maybe if you want something fresh or don't want to bother with any selections, you can just smash Config->Generate random config, which will ask the number of patterns to generate, how often to include a break and what general level of difficulty to use out of 3.  
  
Some more notes:  
The program will adapt to new uniherited timing sections.  
The program might be buggy but should work for the most part, the most dangerous part is high spacing which might lead to stuck patterns and stuff but hopefully still playable.
If you have an idea of what should be added feel free to comment. I might record a video tutorial one day. Have fun!