// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Bildirim fonksiyonları
function loadNotifications() {
    if (!document.getElementById('notificationList')) return;

    $.ajax({
        url: '/Notification/GetUnreadNotifications',
        type: 'GET',
        success: function (notifications) {
            const notificationList = document.getElementById('notificationList');
            notificationList.innerHTML = '';

            if (notifications.length === 0) {
                notificationList.innerHTML = '<div class="notification-item">Yeni bildirim yok</div>';
                return;
            }

            notifications.forEach(function (notification) {
                const notificationItem = document.createElement('div');
                notificationItem.className = 'notification-item' + (notification.isRead ? '' : ' unread');
                notificationItem.innerHTML = `
                    <div class="notification-title">${notification.title}</div>
                    <div class="notification-message">${notification.message}</div>
                    <div class="notification-time">${formatDate(notification.createdAt)}</div>
                `;
                notificationItem.onclick = function () {
                    window.location.href = `/Notification/Details/${notification.id}`;
                };
                notificationList.appendChild(notificationItem);
            });
        }
    });
}

function updateNotificationBadge() {
    $.ajax({
        url: '/Notification/GetUnreadCount',
        type: 'GET',
        success: function (response) {
            const badge = document.querySelector('.notification-badge');
            if (response.count > 0) {
                if (!badge) {
                    const newBadge = document.createElement('span');
                    newBadge.className = 'badge bg-danger notification-badge';
                    newBadge.textContent = response.count;
                    document.querySelector('#notificationDropdown').appendChild(newBadge);
                } else {
                    badge.textContent = response.count;
                }
            } else if (badge) {
                badge.remove();
            }
        }
    });
}

function formatDate(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = Math.floor((now - date) / 1000); // Saniye cinsinden fark

    if (diff < 60) return 'Az önce';
    if (diff < 3600) return Math.floor(diff / 60) + ' dakika önce';
    if (diff < 86400) return Math.floor(diff / 3600) + ' saat önce';
    if (diff < 2592000) return Math.floor(diff / 86400) + ' gün önce';

    return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Sayfa yüklendiğinde bildirimleri yükle
$(document).ready(function () {
    loadNotifications();
    
    // Her 30 saniyede bir bildirimleri güncelle
    setInterval(function () {
        loadNotifications();
        updateNotificationBadge();
    }, 30000);

    // Bildirim dropdown'ı açıldığında bildirimleri yenile
    $('#notificationDropdown').on('show.bs.dropdown', function () {
        loadNotifications();
    });
});
