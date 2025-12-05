
<p align="center"> <img src="./Documentation/LogoMain.png#gh-dark-mode-only" alt="GPU Particle Volumes Main Logo" width="627" /></p>
<p align="center"> <img src="./Documentation/LogoMainBright.png#gh-light-mode-only" alt="GPU Particle Volumes Main Logo" width="627" /></p>

## Overview

This is a **free open-source VRChat Unity tool** that helps to manage GPU particles in your Unity scene. 
This asset comes with one example dust particle shader.

If you want more particle shaders compatible with this system, you can purchase additional shader packs on my Booth and Patreon.

More particle shaders and other assets can be found here:

- **Booth:** https://redsim.booth.pm/  
- **Patreon:** https://www.patreon.com/red_sim  


## Features

- **Particle Volume placement** - define regions where particles **are rendered**
- **Volume Excluder placement** - define regions that **mask / hide particles**
- **Extremely fast GPU-driven particles** - hundreds of thousands of particles can be rendered with **no noticeable performance drop**, even on low-end devices
- **Highly optimized particle mesh format** - particles use **minimal vertex attributes** (positions only)
- **Included particle dust shader** - ready to use out of the box
- **Can be used with VRChat Worlds and Avatars** - avatar usage is not recommended, but possible without the Udon script


## Installation

### **VRC Creator Companion (recommended for VRChat creators)**

1. Open the VPM Listing page: https://redsim.github.io/vpmlisting/
2. Press **“Add to VCC”**
3. Confirm the popup  
   - If no popup appeared, try again (VCC bug)
4. In **VCC → Manage Project**, add the package **“GPU Particle Volumes”**


### **Unity Package Manager (Git URL method)**

1. Unity top menu: **Window -> Package Manager**
2. Press **[ + ]** in the top-left corner
3. Select **“Add package from git URL…”**
4. Paste the Git URL: `https://github.com/REDSIM/GPUParticleVolumes.git?path=/Packages/red.sim.particlevolumes`
5. Press **Add**


## How to Use

1. Drag and drop a particle prefab into your scene.  
2. Select the object and check the **Particle Volume Manager** component.  
3. You will see two lists:
   - **Includers**
   - **Excluders**
4. These lists contain empty GameObjects that represent particle volumes.
5. When assigned and selected, they display special gizmos that help you visualize and resize the volume.  
   (Make sure **Gizmos** are enabled in the Scene View!)
6. **Includers** define where particles will be rendered.
7. **Excluders** define areas where particles are masked  
   (useful for blocking snow/rain inside buildings).
8. **Auto Update Volumes** updates volume transforms during runtime.  
   Enable only if you move or enable/disable volumes at runtime.
9. You can activate or deactivate volumes during runtime.
10. To fully disable the particle system, disable its **Mesh Renderer**.
11. You may use multiple **Particle Volume Manager** components in the scene to support several particle types.
12. Particle count depends on a special particle mesh.  
    (A mesh generator is included in paid asset packs; this free version includes only one example mesh.)
13. Configure particle visuals through the material.


## Support the Project ❤️

If you like this tool, I’d be very happy if you support me on Patreon!  
You’ll get access to many extra assets - including additional GPU Particle Volumes shaders.  
Thank you! <3

Patreon:  
https://www.patreon.com/red_sim  


## Contact

If you have any problems or questions, feel free to DM me on Discord: **RED_SIM**

Additional information and documentation:  
https://github.com/REDSIM/GPUParticleVolumes/
