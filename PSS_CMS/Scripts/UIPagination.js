

    document.addEventListener("DOMContentLoaded", function () {

            const rowsPerPage = 10;

    const table =
    document.querySelector(".tickets-table tbody");

    if (!table) return;

    const rows =
    table.querySelectorAll("tr");

    const pageInfo =
    document.getElementById("pageInfo");

    const prevBtn =
    document.getElementById("prevBtn");

    const nextBtn =
    document.getElementById("nextBtn");

    let currentPage = 1;

    const totalPages =
    Math.ceil(rows.length / rowsPerPage);

    function showPage(page) {

                const start =
    (page - 1) * rowsPerPage;

    const end =
    start + rowsPerPage;

                rows.forEach((row, index) => {

                    if (index >= start && index < end) {

        row.style.display = "";

                    }
    else {

        row.style.display = "none";

                    }

                });

    pageInfo.innerText =
    `Page ${page} of ${totalPages}`;

    prevBtn.disabled =
    page === 1;

    nextBtn.disabled =
    page === totalPages;
            }

    prevBtn.addEventListener("click", function () {

                if (currentPage > 1) {

        currentPage--;

    showPage(currentPage);
                }

            });

    nextBtn.addEventListener("click", function () {

                if (currentPage < totalPages) {

        currentPage++;

    showPage(currentPage);
                }

            });

    showPage(currentPage);

        });

