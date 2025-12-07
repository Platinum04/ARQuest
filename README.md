🧭 ARQuest — Persistent AR Experience at Ilorin Innovation Hub

Welcome to ARQuest, a persistent augmented reality experience built during the Augg.io Persistent AR Hackathon 2025.
Using the Augg.io SDK
, ARQuest transforms the Ilorin Innovation Hub into a living digital space — blending real-world architecture with interactive 3D content and fun facts that celebrate innovation, technology, and local creativity.

📍 Explore. Learn. Experience — Powered by Augg.io

Watch the demo here: 

🎮 Overview

ARQuest turns the Ilorin Innovation Hub into an immersive educational AR experience using persistent anchors.
Visitors can point their phones at the Hub’s outdoor signage or specific areas to discover floating 3D fact markers that tell the story of innovation and growth in Kwara State.

✨ Core Highlights:

🧠 AR Fact Points: Explore interactive fact markers about the Hub.

📍 Persistent Anchors: Content stays fixed to real-world locations.

🌐 Site-Specific Experience: Only accessible at Ilorin Innovation Hub.

🔊 Immersive Audio: Calm background sound for atmosphere.

💫 Seamless Onboarding: Quick splash transition before AR view.

💡 Example AR Facts

Did You Know?
The Ilorin Innovation Hub supports over 50 startups across technology, education, and design.

Innovation Impact
The Hub’s outdoor space is home to community meetups, hackathons, and developer bootcamps for young tech enthusiasts.

Tech for Growth
Founded with the goal of nurturing innovation in northern Nigeria, the Hub provides access to workspace, mentorship, and digital training.

Creative Hub
The Hub’s mission is to drive sustainable innovation and empower the next generation of digital leaders.

🛠️ Built With

Unity 6.2.7f2

Augg.io SDK v1.0.1

AR Foundation (Android)

ARCore Extensions

Custom C# scripts for anchor placement and text rotation

Ambient audio for immersive experience

🧩 Scene Structure
ARQuestScene
├── AR Session
├── AR Session Origin
│   ├── AR Camera
│   ├── Anchor (Parent)
│       ├── 3D Model
│       └── Text Placeholder (Fact)
├── AuggioTrackingManager (Project ID + Location ID)
└── UI_Onboarding (Splash Screen, 3–5s display)

📱 How to Run the Project

Clone this repo:

git clone https://github.com/YOUR-USERNAME/ARQuest.git


Open in Unity Hub → Unity 6.2.7f2.

Import the Augg.io SDK v1.0.1 manually (place in Assets/augg.io/).

Go to Project Settings → XR Plug-in Management → Enable ARCore (Android).

Paste your Augg.io Project ID and Location ID into the AuggioTrackingManager prefab.

Connect your Android phone and select Build & Run.

Head to Ilorin Innovation Hub, point your phone at the signage, and watch ARQuest come alive!

🔊 Sound Design
Event	Audio	Description
Onboarding Splash	intro_whoosh.wav	Soft ambient transition into AR
Anchor Found	anchor_ping.wav	Subtle confirmation ping
Background	hub_ambience.mp3	Light ambient loop inspired by Ilorin
🚀 Future Enhancements

🏆 AR leaderboard for interactive scores and engagement

🗣️ Voice-guided AR storytelling for immersive narration

🧩 Dynamic content via CMS integration for remote updates

🎨 Improved 3D UI and transitions for smoother experience

🧠 Credits

Developer: Awodi Abdulmujeeb Ayomide
Hackathon: Augg.io Persistent AR Challenge 2025
Location: Ilorin Innovation Hub, Kwara State, Nigeria
Powered by: Augg.io

🪄 “ARQuest — where the real world meets digital curiosity.”
