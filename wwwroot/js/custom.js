/* ============================================
   SHOPBILLING CUSTOM JAVASCRIPT
   No Bootstrap Dependencies
   ============================================ */

(function() {
    'use strict';

    // ============================================
    // CUSTOM ALERT/CONFIRM/PROMPT SYSTEM
    // ============================================
    window.customAlert = function(message, title = 'Alert', type = 'info') {
        return new Promise((resolve) => {
            const overlay = document.createElement('div');
            overlay.className = 'custom-alert-overlay show';
            
            const icons = {
                success: 'fa-check-circle',
                error: 'fa-times-circle',
                warning: 'fa-exclamation-triangle',
                info: 'fa-info-circle',
                question: 'fa-question-circle'
            };
            
            overlay.innerHTML = `
                <div class="custom-alert-box">
                    <div class="custom-alert-icon ${type}">
                        <i class="fas ${icons[type] || icons.info}"></i>
                    </div>
                    <h3 class="custom-alert-title">${title}</h3>
                    <p class="custom-alert-message">${message}</p>
                    <div class="custom-alert-buttons">
                        <button class="btn btn-primary alert-ok-btn">OK</button>
                    </div>
                </div>
            `;
            
            document.body.appendChild(overlay);
            document.body.style.overflow = 'hidden';
            
            const okBtn = overlay.querySelector('.alert-ok-btn');
            okBtn.focus();
            
            const closeAlert = () => {
                overlay.classList.remove('show');
                document.body.style.overflow = '';
                setTimeout(() => overlay.remove(), 300);
                resolve(true);
            };
            
            okBtn.addEventListener('click', closeAlert);
            
            overlay.addEventListener('click', (e) => {
                if (e.target === overlay) closeAlert();
            });
            
            document.addEventListener('keydown', function escHandler(e) {
                if (e.key === 'Escape') {
                    document.removeEventListener('keydown', escHandler);
                    closeAlert();
                }
            });
        });
    };

    window.customConfirm = function(message, title = 'Confirm', options = {}) {
        return new Promise((resolve) => {
            const overlay = document.createElement('div');
            overlay.className = 'custom-alert-overlay show';
            
            const confirmText = options.confirmText || 'Confirm';
            const cancelText = options.cancelText || 'Cancel';
            const type = options.type || 'question';
            
            const icons = {
                success: 'fa-check-circle',
                error: 'fa-times-circle',
                warning: 'fa-exclamation-triangle',
                info: 'fa-info-circle',
                question: 'fa-question-circle'
            };
            
            overlay.innerHTML = `
                <div class="custom-alert-box">
                    <div class="custom-alert-icon ${type}">
                        <i class="fas ${icons[type] || icons.question}"></i>
                    </div>
                    <h3 class="custom-alert-title">${title}</h3>
                    <p class="custom-alert-message">${message}</p>
                    <div class="custom-alert-buttons">
                        <button class="btn btn-secondary alert-cancel-btn">${cancelText}</button>
                        <button class="btn btn-primary alert-confirm-btn">${confirmText}</button>
                    </div>
                </div>
            `;
            
            document.body.appendChild(overlay);
            document.body.style.overflow = 'hidden';
            
            const confirmBtn = overlay.querySelector('.alert-confirm-btn');
            const cancelBtn = overlay.querySelector('.alert-cancel-btn');
            confirmBtn.focus();
            
            const closeAlert = (result) => {
                overlay.classList.remove('show');
                document.body.style.overflow = '';
                setTimeout(() => overlay.remove(), 300);
                resolve(result);
            };
            
            confirmBtn.addEventListener('click', () => closeAlert(true));
            cancelBtn.addEventListener('click', () => closeAlert(false));
            
            overlay.addEventListener('click', (e) => {
                if (e.target === overlay) closeAlert(false);
            });
            
            document.addEventListener('keydown', function escHandler(e) {
                if (e.key === 'Escape') {
                    document.removeEventListener('keydown', escHandler);
                    closeAlert(false);
                }
            });
        });
    };

    window.customPrompt = function(message, title = 'Input', defaultValue = '') {
        return new Promise((resolve) => {
            const overlay = document.createElement('div');
            overlay.className = 'custom-alert-overlay show';
            
            overlay.innerHTML = `
                <div class="custom-alert-box">
                    <div class="custom-alert-icon info">
                        <i class="fas fa-edit"></i>
                    </div>
                    <h3 class="custom-alert-title">${title}</h3>
                    <p class="custom-alert-message">${message}</p>
                    <input type="text" class="form-control mb-3" id="promptInput" value="${defaultValue}" />
                    <div class="custom-alert-buttons">
                        <button class="btn btn-secondary alert-cancel-btn">Cancel</button>
                        <button class="btn btn-primary alert-ok-btn">OK</button>
                    </div>
                </div>
            `;
            
            document.body.appendChild(overlay);
            document.body.style.overflow = 'hidden';
            
            const input = overlay.querySelector('#promptInput');
            const okBtn = overlay.querySelector('.alert-ok-btn');
            const cancelBtn = overlay.querySelector('.alert-cancel-btn');
            
            input.focus();
            input.select();
            
            const closeAlert = (result) => {
                overlay.classList.remove('show');
                document.body.style.overflow = '';
                setTimeout(() => overlay.remove(), 300);
                resolve(result);
            };
            
            okBtn.addEventListener('click', () => closeAlert(input.value));
            cancelBtn.addEventListener('click', () => closeAlert(null));
            
            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter') closeAlert(input.value);
            });
            
            overlay.addEventListener('click', (e) => {
                if (e.target === overlay) closeAlert(null);
            });
            
            document.addEventListener('keydown', function escHandler(e) {
                if (e.key === 'Escape') {
                    document.removeEventListener('keydown', escHandler);
                    closeAlert(null);
                }
            });
        });
    };

    // ============================================
    // MOBILE NAVIGATION TOGGLE
    // ============================================
    function initMobileNav() {
        const toggler = document.querySelector('.navbar-toggler');
        const collapse = document.querySelector('.navbar-collapse');
        
        if (toggler && collapse) {
            toggler.addEventListener('click', function() {
                toggler.classList.toggle('active');
                collapse.classList.toggle('show');
            });
            
            // Close menu when clicking outside
            document.addEventListener('click', function(event) {
                const isClickInside = toggler.contains(event.target) || collapse.contains(event.target);
                if (!isClickInside && collapse.classList.contains('show')) {
                    toggler.classList.remove('active');
                    collapse.classList.remove('show');
                }
            });
            
            // Close menu when clicking a nav link
            const navLinks = collapse.querySelectorAll('.nav-link');
            navLinks.forEach(link => {
                link.addEventListener('click', function() {
                    if (window.innerWidth < 992) {
                        toggler.classList.remove('active');
                        collapse.classList.remove('show');
                    }
                });
            });
        }
    }

    // ============================================
    // MODAL SYSTEM
    // ============================================
    window.Modal = {
        show: function(modalId) {
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.classList.add('show');
                document.body.style.overflow = 'hidden';
                
                // Close on backdrop click
                modal.addEventListener('click', function(event) {
                    if (event.target === modal) {
                        Modal.hide(modalId);
                    }
                });
                
                // Close on close button click
                const closeBtn = modal.querySelector('.modal-close');
                if (closeBtn) {
                    closeBtn.addEventListener('click', function() {
                        Modal.hide(modalId);
                    });
                }
                
                // Close on Escape key
                document.addEventListener('keydown', function(event) {
                    if (event.key === 'Escape' && modal.classList.contains('show')) {
                        Modal.hide(modalId);
                    }
                });
            }
        },
        
        hide: function(modalId) {
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.classList.remove('show');
                document.body.style.overflow = '';
            }
        },
        
        toggle: function(modalId) {
            const modal = document.getElementById(modalId);
            if (modal) {
                if (modal.classList.contains('show')) {
                    Modal.hide(modalId);
                } else {
                    Modal.show(modalId);
                }
            }
        }
    };

    // ============================================
    // FORM VALIDATION
    // ============================================
    function initFormValidation() {
        const forms = document.querySelectorAll('form[data-validate]');
        
        forms.forEach(form => {
            form.addEventListener('submit', function(event) {
                let isValid = true;
                const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
                
                inputs.forEach(input => {
                    if (!input.value.trim()) {
                        input.classList.add('is-invalid');
                        input.classList.remove('is-valid');
                        isValid = false;
                    } else {
                        input.classList.remove('is-invalid');
                        input.classList.add('is-valid');
                    }
                });
                
                if (!isValid) {
                    event.preventDefault();
                }
            });
            
            // Real-time validation
            const inputs = form.querySelectorAll('input, select, textarea');
            inputs.forEach(input => {
                input.addEventListener('blur', function() {
                    if (input.hasAttribute('required')) {
                        if (!input.value.trim()) {
                            input.classList.add('is-invalid');
                            input.classList.remove('is-valid');
                        } else {
                            input.classList.remove('is-invalid');
                            input.classList.add('is-valid');
                        }
                    }
                });
                
                input.addEventListener('input', function() {
                    if (input.classList.contains('is-invalid') && input.value.trim()) {
                        input.classList.remove('is-invalid');
                        input.classList.add('is-valid');
                    }
                });
            });
        });
    }

    // ============================================
    // ALERT AUTO-DISMISS
    // ============================================
    function initAlerts() {
        const alerts = document.querySelectorAll('.alert[data-dismiss]');
        
        alerts.forEach(alert => {
            const dismissTime = parseInt(alert.getAttribute('data-dismiss')) || 5000;
            
            setTimeout(() => {
                alert.style.opacity = '0';
                alert.style.transition = 'opacity 0.3s ease';
                
                setTimeout(() => {
                    alert.remove();
                }, 300);
            }, dismissTime);
        });
    }

    // ============================================
    // TOOLTIP SYSTEM (Simple)
    // ============================================
    function initTooltips() {
        const tooltipElements = document.querySelectorAll('[data-tooltip]');
        
        tooltipElements.forEach(element => {
            element.addEventListener('mouseenter', function() {
                const tooltipText = element.getAttribute('data-tooltip');
                const tooltip = document.createElement('div');
                tooltip.className = 'custom-tooltip';
                tooltip.textContent = tooltipText;
                document.body.appendChild(tooltip);
                
                const rect = element.getBoundingClientRect();
                tooltip.style.position = 'absolute';
                tooltip.style.top = (rect.top - tooltip.offsetHeight - 5) + 'px';
                tooltip.style.left = (rect.left + (rect.width / 2) - (tooltip.offsetWidth / 2)) + 'px';
                tooltip.style.background = 'var(--sepia-dark)';
                tooltip.style.color = 'var(--pure-white)';
                tooltip.style.padding = '0.5rem 0.75rem';
                tooltip.style.borderRadius = 'var(--radius-sm)';
                tooltip.style.fontSize = '0.875rem';
                tooltip.style.zIndex = '9999';
                tooltip.style.pointerEvents = 'none';
                
                element._tooltip = tooltip;
            });
            
            element.addEventListener('mouseleave', function() {
                if (element._tooltip) {
                    element._tooltip.remove();
                    element._tooltip = null;
                }
            });
        });
    }

    // ============================================
    // SMOOTH SCROLL
    // ============================================
    function initSmoothScroll() {
        const links = document.querySelectorAll('a[href^="#"]');
        
        links.forEach(link => {
            link.addEventListener('click', function(event) {
                const href = link.getAttribute('href');
                if (href !== '#' && href !== '#!') {
                    const target = document.querySelector(href);
                    if (target) {
                        event.preventDefault();
                        target.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                }
            });
        });
    }

    // ============================================
    // TABLE RESPONSIVE WRAPPER
    // ============================================
    function initResponsiveTables() {
        const tables = document.querySelectorAll('table:not(.table-responsive table)');
        
        tables.forEach(table => {
            if (!table.parentElement.classList.contains('table-responsive')) {
                const wrapper = document.createElement('div');
                wrapper.className = 'table-responsive';
                table.parentNode.insertBefore(wrapper, table);
                wrapper.appendChild(table);
            }
        });
    }

    // ============================================
    // INITIALIZE ALL
    // ============================================
    document.addEventListener('DOMContentLoaded', function() {
        initMobileNav();
        initFormValidation();
        initAlerts();
        initTooltips();
        initSmoothScroll();
        initResponsiveTables();
    });

    // ============================================
    // EXPOSE UTILITIES TO WINDOW
    // ============================================
    window.ShopBilling = {
        Modal: window.Modal,
        initFormValidation: initFormValidation,
        initAlerts: initAlerts
    };

})();
