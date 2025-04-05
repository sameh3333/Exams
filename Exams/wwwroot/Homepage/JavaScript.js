// 🔥 إشعار عند التسجيل بنجاح
document.getElementById('signupForm')?.addEventListener('submit', function (e) {
    e.preventDefault();
    alert('✅ Registered Successfully!');
});

// 🔥 عرض رسم بياني لنتائج الامتحانات باستخدام Chart.js
const ctx = document.getElementById('resultsChart');
if (ctx) {
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Math', 'English', 'Science'],
            datasets: [{
                label: 'Scores',
                data: [80, 90, 70],
                backgroundColor: ['#007bff', '#28a745', '#dc3545']
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: true
                }
            }
        }
    });
}
document.getElementById('examForm')?.addEventListener('submit', function (e) {
    e.preventDefault();
    alert('✅ Your answers have been submitted successfully!');
});
// 🔥 التحقق من الإجابات وحساب النتيجة
document.getElementById('examForm')?.addEventListener('submit', function (e) {
    e.preventDefault();

    let score = 0;
    const correctAnswers = {
        q1: '8',
        q2: '42'
    };

    const formData = new FormData(e.target);
    for (let [question, answer] of formData.entries()) {
        if (answer === correctAnswers[question]) {
            score += 1;
        }
    }

    // تخزين النتيجة في localStorage
    localStorage.setItem('examScore', score);

    // الانتقال إلى صفحة التقييم
    window.location.href = 'result.html';
});

// 🔥 عرض النتيجة في صفحة التقييم
const resultContainer = document.getElementById('result');
if (resultContainer) {
    const score = localStorage.getItem('examScore') || 0;
    const totalQuestions = 2;
    const percentage = (score / totalQuestions) * 100;

    if (percentage >= 50) {
        resultContainer.classList.replace('alert-info', 'alert-success');
        resultContainer.textContent = `👏 Great! You scored ${score} out of ${totalQuestions} (${percentage}%)`;
    } else {
        resultContainer.classList.replace('alert-info', 'alert-danger');
        resultContainer.textContent = `😞 Better luck next time! You scored ${score} out of ${totalQuestions} (${percentage}%)`;
    }

    // مسح النتيجة من localStorage بعد العرض
    localStorage.removeItem('examScore');
}


let questionCount = 1;

// 🔥 إضافة سؤال جديد
function addQuestion() {
    questionCount++;
    const questionHTML = `
        <div class="mb-3">
            <label>${questionCount}. New Question?</label>
            <div class="form-check">
                <input class="form-check-input" type="radio" name="q${questionCount}" required>
                <label class="form-check-label">Option 1</label>
            </div>
            <div class="form-check">
                <input class="form-check-input" type="radio" name="q${questionCount}">
                <label class="form-check-label">Option 2</label>
            </div>
        </div>`;
    document.getElementById('examForm').insertAdjacentHTML('beforeend', questionHTML);
}

// 🔥 حفظ الامتحان وعرض رسالة تأكيد
// document.getElementById('examForm')?.addEventListener('submit', function(e) {
//     e.preventDefault();
//     alert('✅ Exam Saved Successfully!');
//     window.location.href = 'exam-list.html';
// });

//document.getElementById('examForm').addEventListener('submit', function (e) {
//    e.preventDefault();

//    const correctAnswers = {
//        q1: '8',
//        q2: '42'
//    };

//    let score = 0;
//    const totalQuestions = Object.keys(correctAnswers).length;

//    // 🔥 التحقق من الإجابات
//    Object.keys(correctAnswers).forEach(q => {
//        const selectedAnswer = document.querySelector(`input[name="${q}"]:checked`);
//        if (selectedAnswer && selectedAnswer.value === correctAnswers[q]) {
//            score++;
//        }
//    });

//    // 🔥 عرض النتيجة
//    const resultContainer = document.getElementById('resultContainer');
//    resultContainer.classList.remove('d-none');

//    if (score === totalQuestions) {
//        resultContainer.className = 'alert alert-success text-center';
//        resultContainer.textContent = `🎉 Excellent! You scored ${score}/${totalQuestions}`;
//    } else if (score > 0) {
//        resultContainer.className = 'alert alert-warning text-center';
//        resultContainer.textContent = `😊 Good! You scored ${score}/${totalQuestions}`;
//    } else {
//        resultContainer.className = 'alert alert-danger text-center';
//        resultContainer.textContent = `😞 Try Again! You scored ${score}/${totalQuestions}`;
//    }
//});
