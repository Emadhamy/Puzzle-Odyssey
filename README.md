# Puzzle Odyssey - Unity Project

لعبة Puzzle Odyssey متكاملة للأندرويد باستخدام Unity و C#

## نظام المراحل (50 مرحلة)

### أنواع المراحل:
- **المراحل 1-10**: أهداف نقاط (Score Goals) - للتعلم
- **المراحل 11-20**: مكعبات ثلجية (Ice Blocks) - ثابتة وتختفي مع مسح الصف
- **المراحل 21-30**: تحدي الجواهر (Jewel Hunt) - جمع عدد محدد من الجواهر
- **المراحل 31-40**: قنبلة موقوتة (Time Bomb) - يجب مسحها قبل انتهاء العداد
- **المراحل 41-50**: مراحل الزعيم (Boss Levels) - مزيج من كل التحديات + شبكة ممتلئة 40%

## هيكل المشروع

```
Puzzle Odyssey/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GridManager.cs        # إدارة الشبكة 8x8
│   │   │   ├── GridCell.cs           # خلية الشبكة (مع دعم المكعبات الخاصة)
│   │   │   ├── ShapeData.cs          # بيانات الأشكال
│   │   │   └── ShapeController.cs    # التحكم بالسحب والإفلات
│   │   ├── LevelSystem/
│   │   │   ├── LevelData.cs          # بيانات المرحلة (ScriptableObject)
│   │   │   ├── LevelManager.cs       # إدارة شروط الفوز/الخسارة
│   │   │   ├── ProgressManager.cs    # حفظ التقدم (PlayerPrefs)
│   │   │   └── ShapeSpawner.cs       # توليد القطع
│   │   ├── UI/
│   │   │   ├── MainMenu.cs           # القائمة الرئيسية
│   │   │   ├── LevelSelector.cs      # اختيار المرحلة
│   │   │   ├── LevelButton.cs        # زر المرحلة
│   │   │   ├── UIManager.cs          # إدارة واجهة اللعبة
│   │   │   └── ScreenTransition.cs   # انتقالات الشاشات
│   │   └── Utils/
│   │       ├── CameraSetup.cs        # إعداد الكاميرا
│   │       └── GameManager.cs        # إدارة عامة
│   ├── Prefabs/
│   │   ├── GridCell.prefab           # خلية الشبكة
│   │   ├── ShapeController.prefab    # متحكم القطعة
│   │   ├── LevelButton.prefab        # زر اختيار المرحلة
│   │   └── UI/
│   ├── Scenes/
│   │   ├── MainMenu.unity            # القائمة الرئيسية
│   │   ├── LevelSelect.unity         # اختيار المرحلة
│   │   └── GameScene.unity           # مشهد اللعبة
│   └── Resources/
│       └── LevelData/                # بيانات الـ 50 مرحلة
```

## خطوات الإعداد في Unity

### 1. إنشاء Scenes

#### MainMenu Scene:
1. Create New Scene > اسميه "MainMenu"
2. إنشاء Canvas (Screen Space - Overlay)
3. إضافة أزرار: Play, Level Select, Settings, Quit
4. إضافة ScreenTransition
5. Save Scene

#### LevelSelect Scene:
1. Create New Scene > اسميه "LevelSelect"
2. إنشاء Canvas
3. إضافة Panel للأزرار (Grid Layout)
4. إضافة LevelSelector script
5. Save Scene

#### GameScene:
1. Create New Scene > اسميه "GameScene"
2. إضافة GridManager, LevelManager, ShapeSpawner
3. إضافة Canvas مع HUD
4. Save Scene

### 2. إضافة Scenes إلى Build Settings
1. File > Build Settings
2. إضافة المشاهد الثلاثة بالترتيب:
   - MainMenu (0)
   - LevelSelect (1)
   - GameScene (2)

### 3. إنشاء Prefabs

#### GridCell Prefab:
```
GridCell (Empty)
├── Sprite (Background)
├── IceOverlay (Sprite - disabled)
├── JewelOverlay (Sprite - disabled)
├── BombOverlay (Sprite - disabled)
└── BombText (TextMesh)
```
أضف script: GridCell.cs

#### ShapeController Prefab:
```
ShapeController (Empty)
└── [سيتم إنشاء الأشكال ديناميكياً]
```
أضف script: ShapeController.cs

#### LevelButton Prefab:
```
LevelButton (Button)
├── Background (Image)
├── LockIcon (Image)
├── LevelNumber (TextMeshPro)
└── Stars (3 Images)
```
أضف script: LevelButton.cs

### 4. إنشاء LevelData Assets

```csharp
// يمكن إنشاؤها عبر:
// Right Click > Create > Block Puzzle > Level Data
```

مثال Level 1 (Score Goal):
- Level Number: 1
- Level Type: ScoreGoal
- Target Score: 500
- Available Shapes: All enabled

مثال Level 11 (Ice Blocks):
- Level Number: 11
- Level Type: IceBlocks
- Target Score: 1000
- Ice Block Positions: [(2,2), (5,5), ...]

### 5. إعداد المشاهد

#### MainMenu:
```
Main Camera
├── Canvas
│   ├── Title Text
│   ├── Play Button
│   ├── Level Select Button
│   ├── Settings Button
│   └── Quit Button
├── ScreenTransition (Canvas)
└── ProgressManager (Prefab)
```

#### LevelSelect:
```
Main Camera
├── Canvas
│   ├── Title: "Select Level"
│   ├── Buttons Container (Grid)
│   │   └── [LevelButtons سيتم إنشاؤهم ديناميكياً]
│   ├── Navigation (Previous/Next)
│   └── Level Info Panel
├── ScreenTransition
└── LevelSelector (Script)
```

#### GameScene:
```
Main Camera (Orthographic)
├── GridManager
│   └── CellPrefab (assigned)
├── ShapeSpawner
│   ├── ShapeControllerPrefab
│   └── ShapesContainer (Empty)
├── LevelManager
│   └── AllLevels (Array of 50 LevelData)
├── Canvas (HUD)
│   ├── Score Text
│   ├── Level Text
│   ├── Jewels Text (if applicable)
│   ├── Pause Button
│   ├── Pause Menu (Panel)
│   ├── Level Complete Panel
│   └── Game Over Panel
└── ScreenTransition
```

## المميزات

### ✅ نظام المراحل المتكامل
- 50 مرحلة متدرجة الصعوبة
- 5 أنواع مختلفة من التحديات
- نظام قفل/فتح المراحل
- حفظ التقدم تلقائياً

### ✅ أنواع المكعبات الخاصة
1. **Ice Blocks**: ثابتة، تختفي عند مسح الصف/العمود
2. **Jewels**: يجب جمعها للفوز
3. **Time Bombs**: عداد تنازلي، يجب مسحها قبل الانفجار

### ✅ نظام النجوم
- 1-3 نجوم لكل مرحلة
- يعتمد على الأداء (النقاط، عدد الحركات)

### ✅ واجهة مستخدم متكاملة
- شاشة رئيسية
- خريطة المراحل (50 زر)
- HUD أثناء اللعب
- شاشة النجاح (مع نجوم متحركة)
- شاشة الخسارة
- قائمة الإيقاف المؤقت

### ✅ انتقالات ناعمة
- تلاشي (Fade) بين الشاشات
- رسوم متحركة للنجوم
- انتقالات سلسة

## الأوامر

### داخل اللعبة:
- **سحب وإفلات**: لتحريك القطع
- **زر الإيقاف**: فتح قائمة الإيقاف
- **R**: إعادة تشغيل المرحلة (للاختبار)

### في القائمة:
- **Play**: بدء آخر مرحلة مفتوحة
- **Level Select**: اختيار مرحلة
- **Settings**: تعديل الإعدادات

## الإعدادات المحفوظة
- آخر مرحلة مفتوحة
- النجوم المكتسبة لكل مرحلة
- إجمالي النقاط
- إجمالي الجواهر
- إعدادات الصوت
- إعدادات الاهتزاز

## ملاحظات للمطور

### إضافة مرحلة جديدة:
1. إنشاء LevelData asset
2. ضبط الخصائص (النوع، الأهداف، المكعبات الخاصة)
3. إضافة إلى allLevels في LevelManager

### تعديل صعوبة المرحلة:
- تغيير targetScore
- تعديل عدد المكعبات الخاصة
- تغيير الأشكال المتاحة
- تعديل حجم الشبكة

### اختبار سريع:
- استخدم `UnlockAllLevels()` لفتح جميع المراحل
- استخدم `ResetAllProgress()` لإعادة التعيين

## المتطلبات
- Unity 2022.3 LTS أو أحدث
- TextMeshPro package
- Android SDK (للبناء على Android)

## البناء للأندرويد
1. File > Build Settings
2. اختر Android
3. Switch Platform
4. Player Settings:
   - Company Name
   - Product Name
   - Package Name
   - Minimum API Level
5. Build

---

**تم التطوير باستخدام Unity & C#**