// =============================================
// ADMIN DASHBOARD JAVASCRIPT
// =============================================

document.addEventListener('DOMContentLoaded', function() {
    // Initialize admin dashboard
    initSidebar();
    initTooltips();
    initDataTables();
    initCharts();
    initNotifications();
    initFileUploads();
    
    // Auto-dismiss alerts
    setTimeout(function() {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            if (alert.classList.contains('fade')) {
                alert.classList.remove('show');
            }
        });
    }, 5000);
});

// Sidebar functionality
function initSidebar() {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarToggleTop = document.getElementById('sidebarToggleTop');
    const mainContent = document.querySelector('.admin-main');

    // Toggle sidebar
    function toggleSidebar() {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
        
        // Save state to localStorage
        localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
    }

    // Event listeners
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }
    
    if (sidebarToggleTop) {
        sidebarToggleTop.addEventListener('click', function() {
            if (window.innerWidth <= 768) {
                sidebar.classList.toggle('show');
            } else {
                toggleSidebar();
            }
        });
    }

    // Restore sidebar state
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (sidebarCollapsed && window.innerWidth > 768) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
    }

    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth <= 768) {
            sidebar.classList.remove('collapsed');
            mainContent.classList.remove('expanded');
            sidebar.classList.remove('show');
        } else {
            const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
            if (sidebarCollapsed) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            }
        }
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 768) {
            if (!sidebar.contains(e.target) && !sidebarToggleTop.contains(e.target)) {
                sidebar.classList.remove('show');
            }
        }
    });
}

// Initialize tooltips
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Initialize DataTables
function initDataTables() {
    const tables = document.querySelectorAll('.data-table');
    tables.forEach(table => {
        if (typeof DataTable !== 'undefined') {
            new DataTable(table, {
                responsive: true,
                pageLength: 10,
                language: {
                    "lengthMenu": "Hiển thị _MENU_ mục",
                    "zeroRecords": "Không tìm thấy dữ liệu",
                    "info": "Hiển thị _START_ đến _END_ của _TOTAL_ mục",
                    "infoEmpty": "Hiển thị 0 đến 0 của 0 mục",
                    "infoFiltered": "(lọc từ _MAX_ tổng số mục)",
                    "search": "Tìm kiếm:",
                    "paginate": {
                        "first": "Đầu",
                        "last": "Cuối",
                        "next": "Tiếp",
                        "previous": "Trước"
                    }
                }
            });
        }
    });
}

// Initialize charts
function initCharts() {
    // Revenue Chart
    const revenueChart = document.getElementById('revenueChart');
    if (revenueChart && typeof Chart !== 'undefined') {
        const ctx = revenueChart.getContext('2d');
        
        // Get data from data attributes or default data
        const chartData = JSON.parse(revenueChart.dataset.chartData || '[]');
        
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: chartData.map(item => `Tháng ${item.month}/${item.year}`),
                datasets: [{
                    label: 'Doanh thu (VNĐ)',
                    data: chartData.map(item => item.revenue),
                    borderColor: '#3498db',
                    backgroundColor: 'rgba(52, 152, 219, 0.1)',
                    borderWidth: 3,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return new Intl.NumberFormat('vi-VN', {
                                    style: 'currency',
                                    currency: 'VND'
                                }).format(value);
                            }
                        }
                    }
                }
            }
        });
    }

    // Products by Category Chart
    const categoryChart = document.getElementById('categoryChart');
    if (categoryChart && typeof Chart !== 'undefined') {
        const ctx = categoryChart.getContext('2d');
        
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Thời trang nam', 'Thời trang nữ', 'Phụ kiện', 'Giày dép'],
                datasets: [{
                    data: [30, 45, 15, 10],
                    backgroundColor: [
                        '#3498db',
                        '#e74c3c',
                        '#f39c12',
                        '#27ae60'
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    }
}

// Notifications
function initNotifications() {
    // Auto-hide notifications
    const notifications = document.querySelectorAll('.alert');
    notifications.forEach(notification => {
        setTimeout(() => {
            notification.classList.add('fade-out');
            setTimeout(() => {
                notification.remove();
            }, 300);
        }, 5000);
    });
}

// File upload handling
function initFileUploads() {
    const fileInputs = document.querySelectorAll('input[type="file"]');
    
    fileInputs.forEach(input => {
        input.addEventListener('change', function(e) {
            const files = e.target.files;
            const preview = document.getElementById(input.id + 'Preview');
            
            if (preview && files.length > 0) {
                preview.innerHTML = '';
                
                Array.from(files).forEach(file => {
                    if (file.type.startsWith('image/')) {
                        const reader = new FileReader();
                        reader.onload = function(e) {
                            const img = document.createElement('img');
                            img.src = e.target.result;
                            img.classList.add('preview-image');
                            img.style.cssText = 'width: 100px; height: 100px; object-fit: cover; margin: 5px; border-radius: 8px;';
                            preview.appendChild(img);
                        };
                        reader.readAsDataURL(file);
                    }
                });
            }
        });
    });
}

// Utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function formatNumber(num) {
    return new Intl.NumberFormat('vi-VN').format(num);
}

function formatDate(date) {
    return new Date(date).toLocaleDateString('vi-VN');
}

// Confirmation dialogs
function confirmDelete(message = 'Bạn có chắc chắn muốn xóa?') {
    return confirm(message);
}

// AJAX helpers
function ajaxRequest(url, method = 'GET', data = null) {
    return fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: data ? JSON.stringify(data) : null
    });
}

// Loading states
function showLoading(element) {
    if (element) {
        element.classList.add('loading');
        element.style.pointerEvents = 'none';
    }
}

function hideLoading(element) {
    if (element) {
        element.classList.remove('loading');
        element.style.pointerEvents = 'auto';
    }
}

// Toast notifications
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    
    toast.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 5000);
}

// Form validation
function validateForm(form) {
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;
    
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        } else {
            field.classList.remove('is-invalid');
        }
    });
    
    return isValid;
}

// Export functions for global use
window.adminDashboard = {
    formatCurrency,
    formatNumber,
    formatDate,
    confirmDelete,
    ajaxRequest,
    showLoading,
    hideLoading,
    showToast,
    validateForm
};
