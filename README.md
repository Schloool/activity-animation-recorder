# Activity Animation Recorder (AAR)

Activity Animation Recorder is an application used for capturing animations of virtual characters during various activities such as waving or running.

With this tool, you can easily record animations of virtual characters and use them for various purposes such as training AI algorithms to recognize activities. Whether you're working on a research project, developing a video game, or creating digital content, this tool provides a convenient and efficient way to capture animations for your virtual characters.

Some potential applications of this tool include:

- Training AI algorithms for activity recognition
- Creating digital content for video games or films
- Adding animations to virtual or augmented reality experiences

![showcase](https://user-images.githubusercontent.com/18749177/218871526-bbe67785-55b2-4c2b-bb2c-533161eb2192.gif)


## Features
- Record footage of any animation on a character from numerous angles
- Save metadata (currently in XML format) which include camera and rig data for all recordings

## Example dataset (AAR-P01)
An example dataset has been generated under the name of **AAR-P01**. It contains 90,000 Animations of 13 activities based on the most primitive settings possible. The dataset is downloadable for free from [Google Drive](https://drive.google.com/file/d/12DAadhlhZMXoM485hQfvIORvY13gB-3g/view?usp=sharing). Please note that the dataset uses minimalist settings to illustrate the general potential of the application.


## Installation

By default, only two models and animations are included in the build. If you want to include your own assets, please follow these steps:

1. Opening the project in a Unity version of your choice. The recommended version is *Unity 2021.3.14f1*.

2. Load assets into the `Assets/Resources`-folder. 

| Folder       | Context                              | Restrictions                             | Format |
|--------------|--------------------------------------|------------------------------------------|-------|
| `Characters` | Character models used for recordings | Must contain humanoid rig                | FBX   |
| `Animations` | Animations used for recordings       | Must contain at least one animation clip | FBX   |
  
3. Import all assets by using the editor utility (`Import` ➡ `Import All`)

![image](https://user-images.githubusercontent.com/18749177/218715963-515d6528-de84-4ee6-9769-6323467fc724.png)

4. (optional): Configure asset settings manually using the generated Scriptable Objects under `Assets/Scriptable Objects`.

![image](https://github.com/Schloool/activity-animation-recorder/assets/18749177/0396fc12-2019-4e6e-807f-f473e9c063db)

You can set up various details:
- **Clip**: The animation the character will play.
- **Loopable**: Whether the animation clip can be looped or has to be restarted for each recording.
- **Movable**: If set to ``true``, the character can be moved for this animation.
- **Bind actions** allow to attach objects to certain bones of the rig.
  - **Prefab**: The used object.
  - **Time Percentage**: The relative moment of the animation the object will be attached. The value ``0`` means that the object gets attached when the animation starts.
  - **Bone Name**: The name of the rig-bone on which the object will be attached.
  - **Offset**: Relative offset which is applied in relation to the bone in world units.
- **Interact Actions** can be used to make the character interact with other objects in the scene.
  - **Interactable Prefab**: The object that will be generated for the character to interact with.
  - **Spawn Position**: The absolute position at which the object will be set up.
  - **Move Into Object**: If set to ``true``, the character will constantly move into the object during the activity animation.
  - **Turn To Object**: If set to ``true``, the character will turn in the direction of the object.
  - **After Main Animation Clip**: Optional clip which is played by the object after the character has finished their own activity animation.

5. Build the application.

## Usage

The application can be started by opening it directly or using a command line.

### Interface

When using the interface, you can setup several parameters using interface elements:

- Camera settings
  - Amount of cameras
  - Clip length per recording
  - FPS per recording
- Model choice
- Animation choice
- Animation settings
  - Animation speed
  - Enable directed animation (if supported)
  - Directed animation direction (if supported)

![image](https://user-images.githubusercontent.com/18749177/218871880-a1ea0a0e-ccbd-40b0-9ebf-27bb8c2d4e4e.png)

After clicking the button `Record`, all data will be saved inside the folder `Recordings` which will be created inside the build folder.

### Command Line

When starting the application by command line and attaching the flag ``--record``, several startup arguments can be used to configure the recording behaviour.

| Argument         | Default      | Description                                                         |
|------------------|--------------|---------------------------------------------------------------------|
| `cam_amount`     | 5            | Amount of cameras used to record                                    |
| `fps`            | 30           | Amount of Frames Per Second (FPS) used for recording                |
| `clip_length`    | 2.0          | Clip length per camera in seconds                                   |
| `batch_size`     | 7            | The maximum amount of cameras which can be recorded simultaneously  |
| `radius`         | 10.0         | Radius in which cameras will be placed randomly                     |
| `recording_path` | `Recordings` | Folder path in which recordings will be saved                       |
| `metadata_path`  | `Metadata`   | Folder path in which metadata will be saved                         |

**Startup Example:**

```.\recorder.exe --record cam_amount=10 fps=60 clip_length=1.5 radius=5.0 recording_path=Recordings/Videos metadata_path=Recordings/Metadata```

## Future Work

We plan to include more features in the future:
- Dynamic systems for adding scene objects and backgrounds
- Runtime character and animation import tools
- Improved camera and animation settings

## Used third-party apps

- [**MakeHuman**](http://www.makehumancommunity.org/): Used for the models included in the sample application.
- [**Mixamo**](https://www.mixamo.com/#/): Used for the animations included in the sample application.
- [**AVPro Movie Capture**](https://renderheads.com/products/avpro-movie-capture/): Used for the screen recordings. As the version *Basic* is used by default, all newly recorded videos include a watermark. Please see the [pricing plans](https://www.renderheads.com/content/docs/AVProMovieCapture/articles/download.html) for upgrading to a premium version.
