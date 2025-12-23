/**
 * Assignment Dashboard JavaScript
 * Handles functionality for the assignment dashboard including stats cards and actions
 */

document.addEventListener('DOMContentLoaded', function() {
    // Card click handlers for the dashboard stats cards
    setupDashboardCards();
    
    // Setup filter functionality
    setupFilters();
    
    // Setup action buttons
    setupActionButtons();
});

/**
 * Sets up click handlers for the dashboard stats cards
 */
function setupDashboardCards() {
    // Total Assignments card
    const totalAssignmentsCard = document.querySelector('.stat-card:nth-child(1)');
    if (totalAssignmentsCard) {
        totalAssignmentsCard.addEventListener('click', function() {
            // Reset all filters to show all assignments
            const statusFilter = document.getElementById('statusFilter');
            if (statusFilter) statusFilter.value = '';
            
            // Apply filters
            filterAssignments();
        });
    }
    
    // Active Assignments card
    const activeAssignmentsCard = document.querySelector('.stat-card:nth-child(2)');
    if (activeAssignmentsCard) {
        activeAssignmentsCard.addEventListener('click', function() {
            // Set status filter to Active
            const statusFilter = document.getElementById('statusFilter');
            if (statusFilter) statusFilter.value = 'active';
            
            // Apply filters
            filterAssignments();
        });
    }
    
    // Submissions card
    const submissionsCard = document.querySelector('.stat-card:nth-child(3)');
    if (submissionsCard) {
        submissionsCard.addEventListener('click', function() {
            // Sort by submission rate (highest first)
            const sortBy = document.getElementById('sortBy');
            if (sortBy) sortBy.value = 'submissionRate';
            
            // Apply filters
            filterAssignments();
        });
    }
    
    // Overdue Assignments card
    const overdueAssignmentsCard = document.querySelector('.stat-card:nth-child(4)');
    if (overdueAssignmentsCard) {
        overdueAssignmentsCard.addEventListener('click', function() {
            // Set status filter to Overdue
            const statusFilter = document.getElementById('statusFilter');
            if (statusFilter) statusFilter.value = 'overdue';
            
            // Apply filters
            filterAssignments();
        });
    }
}

/**
 * Sets up the filter functionality
 */
function setupFilters() {
    // Course filter change
    const courseFilter = document.getElementById('courseFilter');
    if (courseFilter) {
        courseFilter.addEventListener('change', filterAssignments);
    }
    
    // Status filter change
    const statusFilter = document.getElementById('statusFilter');
    if (statusFilter) {
        statusFilter.addEventListener('change', filterAssignments);
    }
    
    // Sort by change
    const sortBy = document.getElementById('sortBy');
    if (sortBy) {
        sortBy.addEventListener('change', filterAssignments);
    }
    
    // Search functionality
    const searchInput = document.getElementById('searchAssignment');
    const searchButton = searchInput?.nextElementSibling;
    
    if (searchInput) {
        // Search on enter key
        searchInput.addEventListener('keyup', function(event) {
            if (event.key === 'Enter') {
                filterAssignments();
            }
        });
    }
    
    if (searchButton) {
        // Search on button click
        searchButton.addEventListener('click', filterAssignments);
    }
}

/**
 * Sets up action buttons in the assignment table
 */
function setupActionButtons() {
    // Delete assignment confirmation
    const deleteModal = document.getElementById('deleteAssignmentModal');
    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function(event) {
            // Get the button that triggered the modal
            const button = event.relatedTarget;
            
            // Extract info from data attributes
            const assignmentId = button.getAttribute('data-assignment-id');
            const assignmentTitle = button.getAttribute('data-assignment-title');
            
            // Update the modal content
            const assignmentTitleElement = document.getElementById('assignmentTitleToDelete');
            if (assignmentTitleElement) {
                assignmentTitleElement.textContent = assignmentTitle;
            }
            
            // Setup the confirm delete button
            const confirmButton = document.getElementById('confirmDeleteAssignment');
            if (confirmButton) {
                confirmButton.setAttribute('data-assignment-id', assignmentId);
                confirmButton.addEventListener('click', function() {
                    deleteAssignment(assignmentId);
                });
            }
        });
    }
    
    // Confirm delete button
    const confirmDeleteBtn = document.getElementById('confirmDeleteAssignment');
    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener('click', function() {
            const assignmentId = this.getAttribute('data-assignment-id');
            deleteAssignment(assignmentId);
        });
    }
}

/**
 * Filters assignments based on current filter settings
 */
function filterAssignments() {
    const courseFilter = document.getElementById('courseFilter')?.value;
    const statusFilter = document.getElementById('statusFilter')?.value;
    const sortByValue = document.getElementById('sortBy')?.value;
    const searchTerm = document.getElementById('searchAssignment')?.value.toLowerCase();
    
    // Get all assignment rows
    const assignments = document.querySelectorAll('table tbody tr');
    
    // Show/hide assignments based on filters
    assignments.forEach(row => {
        let matchesCourse = true;
        let matchesStatus = true;
        let matchesSearch = true;
        
        // Course filter
        if (courseFilter) {
            const courseName = row.querySelector('td:nth-child(2)').textContent;
            matchesCourse = courseName.includes(courseFilter);
        }
        
        // Status filter
        if (statusFilter) {
            const status = row.querySelector('td:nth-child(6) .badge').textContent.toLowerCase();
            matchesStatus = status === statusFilter.toLowerCase();
        }
        
        // Search filter
        if (searchTerm) {
            const title = row.querySelector('td:nth-child(1) h6').textContent.toLowerCase();
            const description = row.querySelector('td:nth-child(1) small')?.textContent.toLowerCase() || '';
            matchesSearch = title.includes(searchTerm) || description.includes(searchTerm);
        }
        
        // Show/hide row
        if (matchesCourse && matchesStatus && matchesSearch) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
    
    // Sort assignments
    if (sortByValue) {
        sortAssignments(sortByValue);
    }
}

/**
 * Sorts assignments based on the selected sort criteria
 * @param {string} sortBy - The field to sort by
 */
function sortAssignments(sortBy) {
    const tableBody = document.querySelector('table tbody');
    const rows = Array.from(tableBody.querySelectorAll('tr'));
    
    // Sort rows
    rows.sort((a, b) => {
        // Skip hidden rows (they're filtered out)
        if (a.style.display === 'none' || b.style.display === 'none') {
            return 0;
        }
        
        let aValue, bValue;
        
        switch (sortBy) {
            case 'dueDate':
                aValue = new Date(a.querySelector('td:nth-child(3)').textContent);
                bValue = new Date(b.querySelector('td:nth-child(3)').textContent);
                break;
            case 'title':
                aValue = a.querySelector('td:nth-child(1) h6').textContent;
                bValue = b.querySelector('td:nth-child(1) h6').textContent;
                return aValue.localeCompare(bValue);
            case 'course':
                aValue = a.querySelector('td:nth-child(2)').textContent;
                bValue = b.querySelector('td:nth-child(2)').textContent;
                return aValue.localeCompare(bValue);
            case 'submissionRate':
                // Extract submission count and total
                const aText = a.querySelector('td:nth-child(5) span').textContent;
                const bText = b.querySelector('td:nth-child(5) span').textContent;
                
                const [aSubmitted, aTotal] = aText.split('/').map(num => parseInt(num.trim()));
                const [bSubmitted, bTotal] = bText.split('/').map(num => parseInt(num.trim()));
                
                aValue = aTotal > 0 ? (aSubmitted / aTotal) : 0;
                bValue = bTotal > 0 ? (bSubmitted / bTotal) : 0;
                
                // Sort descending by submission rate
                return bValue - aValue;
        }
        
        // Default comparison
        if (aValue < bValue) return -1;
        if (aValue > bValue) return 1;
        return 0;
    });
    
    // Re-append rows in the new order
    rows.forEach(row => tableBody.appendChild(row));
}

/**
 * Delete an assignment
 * @param {number} assignmentId - The ID of the assignment to delete
 */
function deleteAssignment(assignmentId) {
    // In a real app, this would make an API call to delete the assignment
    console.log(`Deleting assignment ${assignmentId}`);
    
    // Show a loading indicator
    const confirmButton = document.getElementById('confirmDeleteAssignment');
    if (confirmButton) {
        confirmButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Deleting...';
        confirmButton.disabled = true;
    }
    
    // Simulate API call with a timeout
    setTimeout(() => {
        // Hide the modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteAssignmentModal'));
        modal.hide();
        
        // Remove the row from the table
        const row = document.querySelector(`tr[data-assignment-id="${assignmentId}"]`);
        if (row) {
            row.remove();
        }
        
        // Show success message
        showAlert('Assignment deleted successfully', 'success');
        
        // Update statistics
        updateStatistics();
        
        // Reset the button
        if (confirmButton) {
            confirmButton.innerHTML = 'Delete Assignment';
            confirmButton.disabled = false;
        }
    }, 1000);
}

/**
 * Update the dashboard statistics after modifications
 */
function updateStatistics() {
    // Get visible (non-filtered) assignments
    const visibleAssignments = Array.from(document.querySelectorAll('table tbody tr')).filter(row => row.style.display !== 'none');
    
    // Total assignments
    const totalAssignments = document.querySelectorAll('table tbody tr').length;
    updateStatCard(1, totalAssignments);
    
    // Active assignments
    const activeAssignments = Array.from(document.querySelectorAll('table tbody tr')).filter(row => {
        const status = row.querySelector('td:nth-child(6) .badge')?.textContent.toLowerCase();
        return status === 'active' || status === 'upcoming';
    }).length;
    updateStatCard(2, activeAssignments);
    
    // Total submissions
    let totalSubmissions = 0;
    document.querySelectorAll('table tbody tr').forEach(row => {
        const submissionText = row.querySelector('td:nth-child(5) span')?.textContent;
        if (submissionText) {
            const [submitted] = submissionText.split('/').map(num => parseInt(num.trim()));
            totalSubmissions += submitted;
        }
    });
    updateStatCard(3, totalSubmissions);
    
    // Overdue assignments
    const overdueAssignments = Array.from(document.querySelectorAll('table tbody tr')).filter(row => {
        const status = row.querySelector('td:nth-child(6) .badge')?.textContent.toLowerCase();
        return status === 'overdue' || status === 'past due';
    }).length;
    updateStatCard(4, overdueAssignments);
}

/**
 * Update a specific stat card value
 * @param {number} cardIndex - The index of the card (1-based)
 * @param {number} value - The new value to display
 */
function updateStatCard(cardIndex, value) {
    const statCard = document.querySelector(`.stat-card:nth-child(${cardIndex}) .stat-card-title`);
    if (statCard) {
        statCard.textContent = value;
    }
}

/**
 * Show an alert message
 * @param {string} message - The message to display
 * @param {string} type - The alert type (success, danger, warning, info)
 */
function showAlert(message, type = 'info') {
    // Create alert element
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.setAttribute('role', 'alert');
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    // Add to the page
    const container = document.querySelector('.container-fluid');
    if (container) {
        container.insertBefore(alertDiv, container.firstChild);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            alertDiv.classList.remove('show');
            setTimeout(() => alertDiv.remove(), 150);
        }, 5000);
    }
} 