# ActionRpgKit

[![Build status](https://ci.appveyor.com/api/projects/status/mqj6fnpo5mml4iq5?svg=true)](https://ci.appveyor.com/project/PaulSchweizer/actionrpgkit) [![Code Climate](https://codeclimate.com/github/PaulSchweizer/ActionRpgKit/badges/gpa.svg)](https://codeclimate.com/github/PaulSchweizer/ActionRpgKit) [![Coverage Status](https://coveralls.io/repos/github/PaulSchweizer/ActionRpgKit/badge.svg)](https://coveralls.io/github/PaulSchweizer/ActionRpgKit)

## Shadowhunter - The Game
The game I created with this library:

![Shadowhunter - The Game](http://www.leifproductions.de/schattenjaeger/game/)

![Game](/docs/game.jpg =250x)
![Menu](/docs/menu.jpg =250x)

## Overview
The Kit is meant to be used within the Unity Game Engine, but is designed to have no dependency to Unity.
This ensures a clean separation of logic and implementation.

## Project Management
The corresponding [Scrum board](https://paulschweizer.atlassian.net/secure/RapidBoard.jspa?projectKey=ARPG&rapidView=4&view=planning.nodetail) and ticket system is hosted on Jira].

Any [strategy documents](https://drive.google.com/open?id=0B_5pqWUPN6WhX29nazlWNjRhN2c) are maintained on Google.

## Documentation
The [Code Documentation](https://paulschweizer.github.io/ActionRpgKit/) is created with doxygen.

## Developing
The development is (mostly) test driven.
[Travis-CI](https://travis-ci.org/PaulSchweizer/ActionRpgKit/branches) is set up to run the unittests with nunit on each push and pull-request.

[Code Climate](https://codeclimate.com/github/PaulSchweizer/ActionRpgKit/badges) is not properly set up as of now.

[Appveyor](https://ci.appveyor.com/project/PaulSchweizer/actionrpgkit) will be set up properly

# Pipeline

## 3D Assets

*Folder Structure inside Unity*
Assets
    Assets
        [AssetName]
            Animation
                [AssetName]@[State]_[OptionalDescription].fbx
            Modeling
                [AssetName].fbx
                    [AssetName] (TopGroup)
                        mixamorig:Hips ... (Joints)
                        GeometryA, GeometryB ... (Geometry)
