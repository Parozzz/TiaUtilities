# TiaUtilities

This project has born out of necessity to decrease time of creating certain part of industrial automation via TIA Portal.

At the moment, this is what is this project:
1) An embedded API to parse SimaticML .xml files generated from TIAOpenness, modify values inside them (Still WIP) and generate a new file to be imported again in TIA. SUPPORT ONLY LADDER LOGIC (LAD).
2) A series of tools to generate ladder fc blocks to automate repetitive task (Alias generation and Alarm generation).
   - A series of grids to better handle repetitive task before done inside TIA Portal
   - Javascript support for better customization
   - Many specific tools to improve QOL
   - Grids support CTRL+Z / CTRL+Y
   - Localization (Still WIP, mixed language for now)
3) AddIn for TIA to import / export blocks to .xml files.

### Mentions
- FastColoredTextBox (For JS Editor) - https://github.com/PavelTorgashov/FastColoredTextBox
- Jint (For JS parsing) - https://github.com/sebastienros/jint
