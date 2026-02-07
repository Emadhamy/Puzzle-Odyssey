# أوامر Git للرفع على GitHub

## 🚀 خطوات الرفع (انسخ والصق في Git Bash)

افتح **Git Bash** في مجلد `Puzzle Odyssey` ثم نفذ هذه الأوامر بالترتيب:

```bash
# 1. انتقل إلى مجلد المشروع (عدل المسار حسب موقع المجلد)
cd "C:/Puzzle Odyssey"

# 2. تهيئة Git
 git init

# 3. إضافة جميع الملفات
 git add .

# 4. إنشاء Commit
 git commit -m "🎮 Initial commit: Puzzle Odyssey game with 50 levels"

# 5. ربط المستودع البعيد
 git remote add origin https://github.com/Emadhamy/Puzzle-Odyssey.git

# 6. رفع الكود إلى GitHub
 git branch -M main
 git push -u origin main
```

---

## ⚠️ إذا ظهرت أخطاء:

### خطأ: "remote origin already exists"
```bash
 git remote remove origin
 git remote add origin https://github.com/Emadhamy/Puzzle-Odyssey.git
```

### خطأ: "failed to push some refs"
```bash
 git pull origin main --rebase
 git push origin main
```

### خطأ: "Authorization failed"
```bash
# استخدم Personal Access Token
 git remote set-url origin https://TOKEN@github.com/Emadhamy/Puzzle-Odyssey.git
```

---

## ✅ التحقق من النجاح

بعد `git push`، افتح الرابط في المتصفح:
```
https://github.com/Emadhamy/Puzzle-Odyssey
```

يجب أن ترى جميع ملفات المشروع!

---

## 🎯 بعد الرفع - تفعيل البناء التلقائي

1. اذهب إلى: https://github.com/Emadhamy/Puzzle-Odyssey
2. انقر على **Settings** → **Secrets and variables** → **Actions**
3. أضف هذه الأسرار:
   - `UNITY_LICENSE` ← انسخ من حسابك في Unity
   - `UNITY_EMAIL` ← بريد Unity
   - `UNITY_PASSWORD` ← كلمة السر
4. اذهب إلى **Actions** → **Build Android APK** → **Run workflow**

**بعد 15 دقيقة، ستجد APK جاهزاً في قسم Releases! 🎉**