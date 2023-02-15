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

4. (optional): Configure asset settings manually using the generated Scriptable Objects under `Assets/Scriptable Objects`. For example, set whether a given animation can be directional or not.

![image](https://user-images.githubusercontent.com/18749177/218716684-00931ddc-dba9-4cb8-a4fc-db75dd570f03.png)

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
- Support for activities related to object (such as using a cellphone)
- Improved camera and animation settings

## Used third-party apps

- [**MakeHuman**](http://www.makehumancommunity.org/): Used for the models included in the sample application.
- [**Mixamo**](https://www.mixamo.com/#/): Used for the animations included in the sample application.
- [**AVPro Movie Capture**](https://renderheads.com/products/avpro-movie-capture/): Used for the screen recordings. As the version *Basic* is used, all recorded videos include a watermark. Please see the [pricing plans](https://www.renderheads.com/content/docs/AVProMovieCapture/articles/download.html) for upgrading to a premium version.