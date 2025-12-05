GPU Particle Volumes by RED_SIM
GitHub: https://github.com/REDSIM/GPUParticleVolumes/

This is a free open source Asset Package which includes few simple GPU particle shaders and prefabs.
However, you can buy more particle shaders, that are compatible with this system, on my Booth and Patreon!

More particle shaders and other assets you can find here:

Booth: https://redsim.booth.pm/
Pateron: https://www.patreon.com/red_sim

HOW TO USE:
- Drag and drop a particle prefab to your scene.
- Look at the "Particle Volume Manager" component.
- There are two lists there: "Includers" and "Excluders".
- They are (empty) game objects that represents volumes for particles.
- When they are specified in Particle Volume Manager, they will have a special volume gizmo to check bounds resize them easily. (Gizmos should be enabled in editor viewport!)
- "Includers" are volumes that specifies when particles will be rendered.
- "Excluders" are volumes that masks particles (Useful to block snow/rain particles to fall through ceiling and walls)
- "Auto Update Volumes" auto updates their position/rotation in runtime. Only enable if you move, enable/disable volumes in runtime.
- You can activate/deactivate includers/excluders volumes in runtime to actually disable them.
- If you want to fully disable the particle system, disable it's Mesh Renderer.
- You can have more than one "Particle Volume Manager" component in your scene if you want to have several different particle types.
- To set actual particle count, you need to have a special particle mesh. Generate it in "Tools -> GPU Particle Volumes -> Simple Particle Mesh Generator"
- To configure the particles visuals, check their material.

HOW TO CHANGE PARTICLE COUNT:
To change particle count, you need to regenerate particle mesh.
Select "Tools -> GPU Particle Volumes -> Simple Particle Mesh Generator"
Choose particles amount and generate the mesh.
After you saved the mesh, apply it to the Mesh Filter on your GPU Particle Volume Manager object.

I'll be very happy if you decide to support me with a patreon subscription! There's a bunch of other cool assets you will get for that, including extra GPU Particle Volumes shaders <3

If you have any problems or questions, be free to DM me on Discord: RED_SIM

More info and instructions here: https://github.com/REDSIM/GPUParticleVolumes/

----------------------------------------------------------------------------------------

GPU Particle Volumes by RED_SIM
GitHub: https://github.com/REDSIM/GPUParticleVolumes/

これは、いくつかのシンプルな GPU パーティクル用シェーダーとプレハブを含む、無料のオープンソースのアセットパッケージです。
また、このシステムと互換性のある追加パーティクルシェーダーを、Booth と Patreon で購入できます！

追加のパーティクルシェーダーやその他のアセットはこちら：

Booth: https://redsim.booth.pm/
Pateron: https://www.patreon.com/red_sim

HOW TO USE:
- パーティクルのプレハブをシーンにドラッグ＆ドロップします。
- "Particle Volume Manager" コンポーネントを確認します。
- そこには "Includers" と "Excluders" の2つのリストがあります。
- これらはパーティクル用ボリュームを表す空のゲームオブジェクトです。
- Particle Volume Manager に設定すると、ボリュームの範囲を調整しやすい専用ギズモが表示されます。（エディタで Gizmos を有効にしてください）
- "Includers" は、パーティクルが表示される領域を示します。
- "Excluders" は、パーティクルをマスクする領域を示します（雪や雨が天井や壁を通過しないようにする際に便利）。
- "Auto Update Volumes" は、ランタイムで位置・回転を自動更新します。ランタイムでボリュームを移動・有効化・無効化する場合のみオンにしてください。
- Includers / Excluders ボリュームは、ランタイム中に有効化・無効化できます。
- パーティクルシステム全体を無効にしたい場合は、Mesh Renderer を無効にしてください。
- 複数種類のパーティクルを使いたい場合、シーンに複数の "Particle Volume Manager" を置くことができます。
- パーティクル数を設定するには専用のパーティクルメッシュが必要です。（有料アセットパックにメッシュジェネレーターが含まれています。デフォルトではサンプルメッシュ1つのみが付属）
- パーティクルの見た目を調整するには、マテリアルを編集してください。

パーティクル数の変更方法:
パーティクル数を変更するには、パーティクル用メッシュを再生成する必要があります。  
「Tools → GPU Particle Volumes → Simple Particle Mesh Generator」を選択します。  
パーティクル数を指定してメッシュを生成します。  
メッシュを保存したら、GPU Particle Volume Manager オブジェクトの Mesh Filter に適用してください。

もしよければ、Patreon で支援していただけるととても嬉しいです！
支援者向けには、追加の GPU Particle Volumes プリセットを含む様々なアセットを提供しています。<3

何か問題や質問があれば、Discord の DM にて気軽にご連絡ください：RED_SIM

その他の情報や説明はこちら： https://github.com/REDSIM/GPUParticleVolumes/
