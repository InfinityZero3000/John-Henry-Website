// ===== ADMIN SCRIPT - ENHANCED VERSION =====

class AdminDashboard {
    constructor() {
        this.init();
        this.bindEvents();
        this.initializeComponents();
    }

    init() {
        this.sidebar = document.getElementById('adminSidebar');
        this.mobileOverlay = document.getElementById('mobileOverlay');
        this.mobileMenuBtn = document.getElementById('mobileMenuBtn');
        this.sidebarToggle = document.getElementById('sidebarToggle');
        this.loadingOverlay = document.getElementById('loadingOverlay');
        
        // Initialize notifications
        this.notifications = [];
        this.checkNotifications();
        
        // Set active nav item
        this.setActiveNavItem();
        
        // Initialize tooltips
        this.initTooltips();
        
        // Auto-hide loading overlay
        setTimeout(() => this.hideLoading(), 1000);
    }

    bindEvents() {
        // Mobile menu toggle
        if (this.mobileMenuBtn) {
            this.mobileMenuBtn.addEventListener('click', () => this.toggleMobileSidebar());
        }

        // Sidebar toggle (mobile)
        if (this.sidebarToggle) {
            this.sidebarToggle.addEventListener('click', () => this.toggleMobileSidebar());
        }

        // Mobile overlay click
        if (this.mobileOverlay) {
            this.mobileOverlay.addEventListener('click', () => this.closeMobileSidebar());
        }

        // Nav link clicks
        this.bindNavLinks();

        // Form submissions
        this.bindForms();

        // Search functionality
        this.bindSearch();

        // Keyboard shortcuts
        this.bindKeyboardShortcuts();

        // Window resize
        window.addEventListener('resize', () => this.handleResize());

        // Before unload
        window.addEventListener('beforeunload', (e) => this.handleBeforeUnload(e));
    }

    bindNavLinks() {
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', (e) => {
                // Add loading state
                if (!link.getAttribute('target')) {
                    this.showLoading();
                }
                
                // Update active state
                this.updateActiveNavItem(link);
            });
        });
    }

    bindForms() {
        const forms = document.querySelectorAll('form[data-ajax="true"]');
        forms.forEach(form => {
            form.addEventListener('submit', (e) => this.handleAjaxForm(e));
        });
    }

    bindSearch() {
        const searchInputs = document.querySelectorAll('[data-search]');
        searchInputs.forEach(input => {
            input.addEventListener('input', (e) => this.handleSearch(e));
        });
    }

    bindKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + K for search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                this.focusSearch();
            }
            
            // Escape to close modals/sidebars
            if (e.key === 'Escape') {
                this.closeMobileSidebar();
                this.closeModals();
            }
            
            // Alt + D for dashboard
            if (e.altKey && e.key === 'd') {
                e.preventDefault();
                window.location.href = '/Admin/Dashboard';
            }
        });
    }

    // ===== SIDEBAR MANAGEMENT =====
    toggleMobileSidebar() {
        if (this.sidebar && this.mobileOverlay) {
            this.sidebar.classList.toggle('show');
            this.mobileOverlay.classList.toggle('show');
            document.body.classList.toggle('sidebar-open');
        }
    }

    closeMobileSidebar() {
        if (this.sidebar && this.mobileOverlay) {
            this.sidebar.classList.remove('show');
            this.mobileOverlay.classList.remove('show');
            document.body.classList.remove('sidebar-open');
        }
    }

    setActiveNavItem() {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.nav-link');
        
        navLinks.forEach(link => {
            link.classList.remove('active');
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
                
                // Expand parent group if collapsed
                const navGroup = link.closest('.nav-group');
                if (navGroup) {
                    navGroup.classList.add('active');
                }
            }
        });
    }

    updateActiveNavItem(clickedLink) {
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => link.classList.remove('active'));
        clickedLink.classList.add('active');
    }

    // ===== LOADING MANAGEMENT =====
    showLoading() {
        if (this.loadingOverlay) {
            this.loadingOverlay.style.display = 'flex';
        }
    }

    hideLoading() {
        if (this.loadingOverlay) {
            this.loadingOverlay.style.display = 'none';
        }
    }

    // ===== NOTIFICATIONS =====
    checkNotifications() {
        // Simulate API call for notifications
        this.notifications = [
            {
                id: 1,
                title: 'Đơn hàng mới #1234',
                message: 'Có đơn hàng mới cần xử lý',
                type: 'order',
                time: '5 phút trước',
                unread: true
            },
            {
                id: 2,
                title: 'Sản phẩm sắp hết hàng',
                message: 'Áo polo trắng chỉ còn 3 sản phẩm',
                type: 'inventory',
                time: '15 phút trước',
                unread: true
            },
            {
                id: 3,
                title: 'Đánh giá mới',
                message: 'Khách hàng vừa đánh giá 5 sao',
                type: 'review',
                time: '1 giờ trước',
                unread: false
            }
        ];

        this.updateNotificationBadge();
    }

    updateNotificationBadge() {
        const unreadCount = this.notifications.filter(n => n.unread).length;
        const badge = document.querySelector('.notification-badge');
        if (badge) {
            badge.textContent = unreadCount;
            badge.style.display = unreadCount > 0 ? 'block' : 'none';
        }
    }

    markNotificationAsRead(notificationId) {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification) {
            notification.unread = false;
            this.updateNotificationBadge();
        }
    }

    // ===== FORM HANDLING =====
    handleAjaxForm(event) {
        event.preventDefault();
        const form = event.target;
        const formData = new FormData(form);
        const url = form.action;
        const method = form.method || 'POST';

        this.showLoading();

        fetch(url, {
            method: method,
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.json())
        .then(data => {
            this.hideLoading();
            
            if (data.success) {
                this.showToast('Thành công!', data.message, 'success');
                
                // Redirect if specified
                if (data.redirectUrl) {
                    setTimeout(() => {
                        window.location.href = data.redirectUrl;
                    }, 1500);
                }
                
                // Refresh table if specified
                if (data.refreshTable) {
                    this.refreshTable();
                }
            } else {
                this.showToast('Lỗi!', data.message, 'error');
            }
        })
        .catch(error => {
            this.hideLoading();
            this.showToast('Lỗi!', 'Có lỗi xảy ra khi xử lý yêu cầu', 'error');
            console.error('Error:', error);
        });
    }

    // ===== SEARCH FUNCTIONALITY =====
    handleSearch(event) {
        const searchTerm = event.target.value.toLowerCase();
        const targetTable = event.target.getAttribute('data-search');
        const table = document.querySelector(targetTable);
        
        if (!table) return;

        const rows = table.querySelectorAll('tbody tr');
        let visibleCount = 0;

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const isVisible = text.includes(searchTerm);
            
            row.style.display = isVisible ? '' : 'none';
            if (isVisible) visibleCount++;
        });

        // Update search results info
        this.updateSearchResults(visibleCount, rows.length);
    }

    updateSearchResults(visible, total) {
        const resultsInfo = document.querySelector('.search-results-info');
        if (resultsInfo) {
            resultsInfo.textContent = `Hiển thị ${visible} / ${total} kết quả`;
        }
    }

    focusSearch() {
        const searchInput = document.querySelector('.global-search');
        if (searchInput) {
            searchInput.focus();
        }
    }

    // ===== MODAL MANAGEMENT =====
    closeModals() {
        const modals = document.querySelectorAll('.modal.show');
        modals.forEach(modal => {
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }
        });
    }

    // ===== TABLE MANAGEMENT =====
    refreshTable() {
        const tables = document.querySelectorAll('[data-refresh="true"]');
        tables.forEach(table => {
            // Simulate table refresh
            this.showLoading();
            setTimeout(() => {
                this.hideLoading();
                this.showToast('Thành công!', 'Đã cập nhật dữ liệu', 'success');
            }, 1000);
        });
    }

    // ===== BULK ACTIONS =====
    initializeBulkActions() {
        const selectAll = document.querySelector('.select-all');
        const itemCheckboxes = document.querySelectorAll('.item-checkbox');
        const bulkActionsBar = document.querySelector('.bulk-actions-bar');

        if (selectAll) {
            selectAll.addEventListener('change', () => {
                itemCheckboxes.forEach(checkbox => {
                    checkbox.checked = selectAll.checked;
                });
                this.updateBulkActionsBar();
            });
        }

        itemCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', () => {
                this.updateBulkActionsBar();
            });
        });
    }

    updateBulkActionsBar() {
        const checkedItems = document.querySelectorAll('.item-checkbox:checked');
        const bulkActionsBar = document.querySelector('.bulk-actions-bar');
        const selectedCount = document.querySelector('.selected-count');

        if (bulkActionsBar) {
            bulkActionsBar.style.display = checkedItems.length > 0 ? 'flex' : 'none';
        }

        if (selectedCount) {
            selectedCount.textContent = checkedItems.length;
        }
    }

    // ===== CHARTS INITIALIZATION =====
    initializeCharts() {
        // Sales Chart
        const salesChartCtx = document.getElementById('salesChart');
        if (salesChartCtx) {
            new Chart(salesChartCtx, {
                type: 'line',
                data: {
                    labels: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6'],
                    datasets: [{
                        label: 'Doanh thu',
                        data: [12000000, 15000000, 18000000, 16000000, 22000000, 25000000],
                        borderColor: '#667eea',
                        backgroundColor: 'rgba(102, 126, 234, 0.1)',
                        tension: 0.4,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
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
                                    return new Intl.NumberFormat('vi-VN').format(value) + ' ₫';
                                }
                            }
                        }
                    }
                }
            });
        }

        // Orders Chart
        const ordersChartCtx = document.getElementById('ordersChart');
        if (ordersChartCtx) {
            new Chart(ordersChartCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Hoàn thành', 'Đang xử lý', 'Đã hủy'],
                    datasets: [{
                        data: [65, 25, 10],
                        backgroundColor: ['#28a745', '#ffc107', '#dc3545'],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    }

    // ===== TOOLTIP INITIALIZATION =====
    initTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // ===== COMPONENTS INITIALIZATION =====
    initializeComponents() {
        this.initializeBulkActions();
        this.initializeCharts();
        this.initializeCounters();
        this.initializeProgressBars();
    }

    initializeCounters() {
        const counters = document.querySelectorAll('.counter');
        counters.forEach(counter => {
            const target = parseInt(counter.getAttribute('data-target'));
            const duration = 2000;
            const increment = target / (duration / 16);
            let current = 0;

            const updateCounter = () => {
                current += increment;
                if (current < target) {
                    counter.textContent = Math.floor(current).toLocaleString('vi-VN');
                    requestAnimationFrame(updateCounter);
                } else {
                    counter.textContent = target.toLocaleString('vi-VN');
                }
            };

            // Start counter when element is visible
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        updateCounter();
                        observer.unobserve(entry.target);
                    }
                });
            });

            observer.observe(counter);
        });
    }

    initializeProgressBars() {
        const progressBars = document.querySelectorAll('.progress-bar-animated');
        progressBars.forEach(bar => {
            const width = bar.getAttribute('data-width');
            setTimeout(() => {
                bar.style.width = width + '%';
            }, 500);
        });
    }

    // ===== TOAST NOTIFICATIONS =====
    showToast(title, message, type = 'info') {
        const toastContainer = this.getOrCreateToastContainer();
        const toast = this.createToast(title, message, type);
        
        toastContainer.appendChild(toast);
        
        // Show toast
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
        
        // Auto remove after delay
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 5000);
    }

    getOrCreateToastContainer() {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
        return container;
    }

    createToast(title, message, type) {
        const typeColors = {
            success: 'bg-success',
            error: 'bg-danger',
            warning: 'bg-warning',
            info: 'bg-info'
        };

        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white ${typeColors[type] || typeColors.info} border-0`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    <strong>${title}</strong><br>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;
        return toast;
    }

    // ===== UTILITY METHODS =====
    handleResize() {
        // Handle responsive behavior
        if (window.innerWidth > 768) {
            this.closeMobileSidebar();
        }
    }

    handleBeforeUnload(event) {
        // Check for unsaved changes
        const unsavedForms = document.querySelectorAll('form[data-unsaved="true"]');
        if (unsavedForms.length > 0) {
            event.preventDefault();
            event.returnValue = 'Bạn có thay đổi chưa được lưu. Bạn có chắc muốn rời khỏi trang?';
        }
    }

    // ===== FILE UPLOAD =====
    initializeFileUploads() {
        const fileInputs = document.querySelectorAll('input[type="file"][data-preview]');
        fileInputs.forEach(input => {
            input.addEventListener('change', (e) => this.handleFilePreview(e));
        });
    }

    handleFilePreview(event) {
        const file = event.target.files[0];
        const previewContainer = document.querySelector(event.target.getAttribute('data-preview'));
        
        if (file && previewContainer) {
            const reader = new FileReader();
            reader.onload = (e) => {
                if (file.type.startsWith('image/')) {
                    previewContainer.innerHTML = `<img src="${e.target.result}" class="img-fluid rounded" alt="Preview">`;
                } else {
                    previewContainer.innerHTML = `<div class="file-preview"><i class="fas fa-file"></i> ${file.name}</div>`;
                }
            };
            reader.readAsDataURL(file);
        }
    }

    // ===== DATA EXPORT =====
    exportData(format, data, filename) {
        if (format === 'csv') {
            this.exportToCSV(data, filename);
        } else if (format === 'excel') {
            this.exportToExcel(data, filename);
        } else if (format === 'pdf') {
            this.exportToPDF(data, filename);
        }
    }

    exportToCSV(data, filename) {
        const csv = this.convertToCSV(data);
        const blob = new Blob([csv], { type: 'text/csv' });
        this.downloadFile(blob, filename + '.csv');
    }

    convertToCSV(data) {
        if (!data || data.length === 0) return '';
        
        const headers = Object.keys(data[0]);
        const csvContent = [
            headers.join(','),
            ...data.map(row => headers.map(header => `"${row[header] || ''}"`).join(','))
        ].join('\n');
        
        return csvContent;
    }

    downloadFile(blob, filename) {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }
}

// ===== GLOBAL FUNCTIONS =====
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function formatDate(date) {
    return new Intl.DateTimeFormat('vi-VN').format(new Date(date));
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// ===== INITIALIZATION =====
document.addEventListener('DOMContentLoaded', function() {
    // Initialize admin dashboard
    window.adminDashboard = new AdminDashboard();
    
    // Initialize file uploads
    window.adminDashboard.initializeFileUploads();
    
    // Auto-refresh data every 5 minutes
    setInterval(() => {
        window.adminDashboard.checkNotifications();
    }, 300000);
    
    // Add loading states to all forms
    const forms = document.querySelectorAll('form:not([data-ajax])');
    forms.forEach(form => {
        form.addEventListener('submit', () => {
            window.adminDashboard.showLoading();
        });
    });
});

// ===== EXPORT FOR EXTERNAL USE =====
window.AdminUtils = {
    showToast: (title, message, type) => window.adminDashboard.showToast(title, message, type),
    showLoading: () => window.adminDashboard.showLoading(),
    hideLoading: () => window.adminDashboard.hideLoading(),
    exportData: (format, data, filename) => window.adminDashboard.exportData(format, data, filename),
    refreshTable: () => window.adminDashboard.refreshTable()
};