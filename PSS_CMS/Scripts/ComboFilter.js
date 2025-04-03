
    $(document).ready(function () {
            // Retrieve stored values from localStorage
            var storedTicketType = localStorage.getItem("selectedTicketType");
    var storedProjectType = localStorage.getItem("selectedProjectType");

    if (storedTicketType) {
        $("#ticketTypeFilter").val(storedTicketType);
            }
    if (storedProjectType) {
        $("#projectTypeFilter").val(storedProjectType);
            }

    // Apply filter on page load
    filterTable(storedTicketType, storedProjectType);

    $("#ticketTypeFilter, #projectTypeFilter").change(function () {
                var selectedTicketType = $("#ticketTypeFilter").val();
    var selectedProjectType = $("#projectTypeFilter").val();
        console.log("pval", selectedProjectType)
        console.log("tval", selectedTicketType)
    // Store selected values in localStorage
    localStorage.setItem("selectedTicketType", selectedTicketType);
    localStorage.setItem("selectedProjectType", selectedProjectType);

    filterTable(selectedTicketType, selectedProjectType);
            });

        function filterTable(ticketType, projectType) {
            var filterTicketType = ticketType ? ticketType.trim().toLowerCase() : "";
            var filterProjectType = projectType ? projectType.trim().toLowerCase() : "";
            var rowCount = 0;

            console.log("Filtering for:");
            console.log("Ticket Type:", filterTicketType);
            console.log("Project Type:", filterProjectType);

            $("#ticketTable tbody tr").each(function () {
                var rowTicketType = $(this).find("td:nth-child(4)").text().trim().toLowerCase();
                var rowProjectType = $(this).find("td:nth-child(2)").text().trim().toLowerCase();

                console.log("Row Data - Ticket Type:", rowTicketType, "| Project Type:", rowProjectType);

                var ticketTypeMatches = filterTicketType === "" || rowTicketType === filterTicketType;
                var projectTypeMatches = filterProjectType === "" || rowProjectType === filterProjectType;

                if ((filterTicketType && filterProjectType && ticketTypeMatches && projectTypeMatches) ||
                    (filterTicketType && !filterProjectType && ticketTypeMatches) ||
                    (!filterTicketType && filterProjectType && projectTypeMatches) ||
                    (!filterTicketType && !filterProjectType)) {
                    $(this).show();
                    rowCount++;
                } else {
                    $(this).hide();
                }
            });

            if (rowCount === 0) {
                if ($("#noDataMessage").length === 0) {
                    $("#ticketTable tbody").append("<tr id='noDataMessage'><td colspan='6' class='text-center text-danger'>No matching records found</td></tr>");
                }
            } else {
                $("#noDataMessage").remove();
            }
        }

        });
