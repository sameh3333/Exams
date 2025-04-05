let questionCount = 1;

// 🔥 إضافة سؤال جديد
function addQuestion() {
    questionCount++;
    const questionHTML = `
        <div class="question mb-4 border p-3 rounded">
            <label class="form-label">${questionCount}. Question:</label>
            <input type="text" class="form-control mb-2" name="question${questionCount}" placeholder="Enter your question" required>

            <label class="form-label">Choices:</label>
            <div class="mb-2">
                <input type="text" class="form-control mb-1" name="q${questionCount}_choice1" placeholder="Choice 1" required>
                <input type="text" class="form-control mb-1" name="q${questionCount}_choice2" placeholder="Choice 2" required>
                <input type="text" class="form-control mb-1" name="q${questionCount}_choice3" placeholder="Choice 3" required>
                <input type="text" class="form-control mb-1" name="q${questionCount}_choice4" placeholder="Choice 4" required>
            </div>

            <label class="form-label">Correct Answer:</label>
            <select class="form-select mb-2" name="q${questionCount}_correct" required>
                <option value="" disabled selected>Select correct answer</option>
                <option value="1">Choice 1</option>
                <option value="2">Choice 2</option>
                <option value="3">Choice 3</option>
                <option value="4">Choice 4</option>
            </select>
        </div>`;
    document.getElementById('questionsContainer').insertAdjacentHTML('beforeend', questionHTML);
}

// 🔥 حفظ الامتحان وعرض رسالة تأكيد
document.getElementById('examForm')?.addEventListener('submit', function (e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const examData = {};

    formData.forEach((value, key) => {
        examData[key] = value;
    });

    console.log("Exam Data:", examData);  // ✅ عرض بيانات الامتحان في الكونسول

    alert('✅ Exam Saved Successfully!');
    e.target.reset();  // 🔥 إعادة تعيين النموذج بعد الحفظ
});
