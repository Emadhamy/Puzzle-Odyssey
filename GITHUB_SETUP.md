# GitHub Setup Guide for Puzzle Odyssey

## 🚀 خطوات رفع المشروع على GitHub

### 1. إنشاء مستودع جديد على GitHub

1. اذهب إلى [GitHub](https://github.com)
2. انقر على **New Repository**
3. املأ البيانات:
   - **Repository name**: `puzzle-odyssey`
   - **Description**: `Puzzle Odyssey - A challenging block puzzle game with 50 levels`
   - **Public** أو **Private** (حسب رغبتك)
   - **☑** Initialize with README (اختياري)
4. انقر **Create repository**

### 2. رفع الكود المحلي

افتح **Git Bash** أو **Command Prompt** داخل مجلد `Puzzle Odyssey`:

```bash
# تهيئة Git
 git init

# إضافة جميع الملفات
 git add .

# أول Commit
 git commit -m "Initial commit: Puzzle Odyssey game with 50 levels"

# ربط المستودع البعيد
 git remote add origin https://github.com/YOUR_USERNAME/puzzle-odyssey.git

# رفع الكود
 git push -u origin main
```

### 3. إعداد GitHub Actions (للبناء التلقائي)

#### 3.1 الحصول على Unity License

1. اذهب إلى [Unity ID](https://id.unity.com)
2. سجل الدخول بحسابك
3. اذهب إلى **My Seats** → **Manage licenses**
4. انسخ **Unity License** الخاص بك

#### 3.2 إضافة Secrets إلى GitHub

1. في مستودع GitHub، اذهب إلى **Settings**
2. انقر على **Secrets and variables** → **Actions**
3. أضف هذه الأسرار:

```
UNITY_LICENSE=-----BEGIN UNITY LICENSE-----
[الترخيص الخاص بك]
-----END UNITY LICENSE-----

UNITY_EMAIL=your-email@example.com
UNITY_PASSWORD=your-unity-password
```

#### 3.3 تشغيل Workflow

1. اذهب إلى **Actions** تبويب
2. ستجد workflow **Build Android APK**
3. انقر على **Run workflow**
4. انتظر حتى ينتهي البناء (قد يستغرق 10-20 دقيقة)
5. بعد النجاح، ستجد APK في قسم **Releases**

---

## 📱 طريقة أخرى: رفع APK يدوياً

إذا لم ترد استخدام GitHub Actions:

### 1. بناء APK محلياً
```
Unity → File → Build Settings → Android → Build
```

### 2. إنشاء Release على GitHub
1. اذهب إلى **Releases** → **Draft a new release**
2. اختر **Tag**: `v1.0.0`
3. **Title**: `Puzzle Odyssey v1.0.0`
4. **Description**: اكتب تفاصيل الإصدار
5. اسحب ملف APK إلى **Attach binaries**
6. انقر **Publish release**

---

## 🔧 حل المشكلات الشائعة

### مشكلة: "Library folder is too large"
```bash
# تأكد من وجود .gitignore
 git rm -r --cached Library
 git rm -r --cached Temp
 git rm -r --cached Obj
 git commit -m "Remove cached folders"
```

### مشكلة: "Authentication failed"
```bash
# استخدم Personal Access Token
 git remote set-url origin https://TOKEN@github.com/YOUR_USERNAME/puzzle-odyssey.git
```

### مشكلة: "Large file warning"
```bash
# تثبيت Git LFS
 git lfs install
 git lfs track "*.psd"
 git lfs track "*.png"
 git lfs track "*.jpg"
 git add .gitattributes
```

---

## 📋 Checklist قبل الرفع

- [ ] المشروع يعمل محلياً
- [ ] جميع Scenes مضافة إلى Build Settings
- [ ] `.gitignore` موجود
- [ ] `README.md` مُحدّث
- [ ] لا يوجد ملفات كبيرة (تحقق من Library/, Temp/)
- [ ] جميع Assets في مجلد Assets/
- [ ] لا توجد أخطاء في Console

---

## 🎯 بعد الرفع

بمجرد رفع المشروع:
1. شارك رابط GitHub مع الآخرين
2. يمكنهم Clone المشروع وتشغيله في Unity
3. GitHub Actions سيبني APK تلقائياً
4. تجد APK في قسم Releases

---

## 🔗 روابط مفيدة

- [Unity Cloud Build](https://unity.com/products/cloud-build)
- [GitHub Actions for Unity](https://game.ci/docs/github/getting-started)
- [Git LFS](https://git-lfs.github.com/)

**بالتوفيق! 🚀**