    let currentQuestionIndex = 0;
    const totalQuestions = @Model.Questions.Count;
    const questions = document.querySelectorAll(".question-container");

        // ✅ Next button with validation
        document.querySelectorAll(".next-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            let index = parseInt(this.getAttribute("data-index"));
            let currentQuestion = questions[index];

            // check if user selected answer
            let selected = currentQuestion.querySelector("input[type=radio]:checked");
            if (!selected) {
                // لو المستخدم ما اختارش إجابة
                alert("⚠️ Please select an answer before moving to the next question.");
                return;
            }

            // move to next
            questions[index].classList.add("d-none");
            questions[index + 1].classList.remove("d-none");
            currentQuestionIndex = index + 1;
            updateNavButtons();
        });
        });

        // ✅ Previous button (no validation)
        document.querySelectorAll(".prev-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            let index = parseInt(this.getAttribute("data-index"));
            questions[index].classList.add("d-none");
            questions[index - 1].classList.remove("d-none");
            currentQuestionIndex = index - 1;
            updateNavButtons();
        });
        });

    function updateNavButtons() {
        document.getElementById("submit-btn").style.display =
        (currentQuestionIndex === totalQuestions - 1) ? "block" : "none";
        }

        // ✅ Timer
        setInterval(() => {
            const timerDisplay = document.getElementById("exam-timer");
    let time = timerDisplay.textContent.split(":");
    let minutes = parseInt(time[0]);
    let seconds = parseInt(time[1]);
    seconds++;
    if (seconds === 60) {minutes++; seconds = 0; }
    timerDisplay.textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
        }, 1000);

    // ✅ Final validation before submit
    document.getElementById("exam-form").addEventListener("submit", function (e) {
        let allAnswered = true;
            questions.forEach(q => {
                if (!q.querySelector("input[type=radio]:checked")) {
        allAnswered = false;
                }
            });

    if (!allAnswered) {
        e.preventDefault();
    alert("⚠️ Please answer all questions before submitting the exam.");
            }
        });
