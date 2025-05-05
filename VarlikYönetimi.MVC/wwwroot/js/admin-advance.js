document.addEventListener('DOMContentLoaded', function() {
    // Kart animasyonları
    const cards = document.querySelectorAll('.advance-card');
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });

    // Onaylama işlemi
    const approveButtons = document.querySelectorAll('.btn-approve');
    approveButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            if (confirm('Bu avans talebini onaylamak istediğinizden emin misiniz?')) {
                const requestId = this.getAttribute('data-id');
                approveRequest(requestId);
            }
        });
    });

    // Reddetme işlemi
    const rejectButtons = document.querySelectorAll('.btn-reject');
    rejectButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            if (confirm('Bu avans talebini reddetmek istediğinizden emin misiniz?')) {
                const requestId = this.getAttribute('data-id');
                rejectRequest(requestId);
            }
        });
    });

    // Detay butonları için event listener
    const detailButtons = document.querySelectorAll('.btn-details');
    detailButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const url = this.getAttribute('href');
            if (url) {
                window.location.href = url;
            }
        });
    });

    // Detay sayfası animasyonları
    const detailCards = document.querySelectorAll('.card');
    detailCards.forEach((card, index) => {
        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });
});

// Avans talebini onaylama fonksiyonu
async function approveRequest(requestId) {
    try {
        const response = await fetch(`/Admin/AdvanceRequests/Approve/${requestId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (response.ok) {
            showNotification('Avans talebi başarıyla onaylandı', 'success');
            setTimeout(() => {
                window.location.reload();
            }, 1500);
        } else {
            showNotification('Avans talebi onaylanırken bir hata oluştu', 'error');
        }
    } catch (error) {
        showNotification('Bir hata oluştu', 'error');
        console.error('Hata:', error);
    }
}

// Avans talebini reddetme fonksiyonu
async function rejectRequest(requestId) {
    try {
        const response = await fetch(`/Admin/AdvanceRequests/Reject/${requestId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (response.ok) {
            showNotification('Avans talebi başarıyla reddedildi', 'success');
            setTimeout(() => {
                window.location.reload();
            }, 1500);
        } else {
            showNotification('Avans talebi reddedilirken bir hata oluştu', 'error');
        }
    } catch (error) {
        showNotification('Bir hata oluştu', 'error');
        console.error('Hata:', error);
    }
}

// Avans talebi detaylarını görüntüleme fonksiyonu
async function showRequestDetails(requestId) {
    try {
        const response = await fetch(`/Admin/AdvanceRequests/Details/${requestId}`);
        if (response.ok) {
            const data = await response.json();
            // Modal içeriğini oluştur ve göster
            const modal = document.createElement('div');
            modal.className = 'modal fade';
            modal.id = 'requestDetailsModal';
            modal.innerHTML = `
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Avans Talebi Detayları</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <p><strong>Talep Eden:</strong> ${data.requesterName}</p>
                                    <p><strong>Talep Tarihi:</strong> ${new Date(data.requestDate).toLocaleDateString('tr-TR')}</p>
                                    <p><strong>İstenen Tarih:</strong> ${new Date(data.desiredDate).toLocaleDateString('tr-TR')}</p>
                                </div>
                                <div class="col-md-6">
                                    <p><strong>Miktar:</strong> ${data.amount} TL</p>
                                    <p><strong>Durum:</strong> ${data.status}</p>
                                    <p><strong>Açıklama:</strong> ${data.description || 'Açıklama yok'}</p>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
            const modalInstance = new bootstrap.Modal(modal);
            modalInstance.show();
            modal.addEventListener('hidden.bs.modal', function () {
                modal.remove();
            });
        } else {
            showNotification('Detaylar yüklenirken bir hata oluştu', 'error');
        }
    } catch (error) {
        showNotification('Bir hata oluştu', 'error');
        console.error('Hata:', error);
    }
}

// Bildirim gösterme fonksiyonu
function showNotification(message, type) {
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    document.body.appendChild(toast);
    const toastInstance = new bootstrap.Toast(toast);
    toastInstance.show();
    toast.addEventListener('hidden.bs.toast', function () {
        toast.remove();
    });
}

// Onaylama ve reddetme işlemleri için onay kutusu
function confirmAction(action, id) {
    const message = action === 'approve' ? 
        'Bu avans talebini onaylamak istediğinizden emin misiniz?' : 
        'Bu avans talebini reddetmek istediğinizden emin misiniz?';
    
    if (confirm(message)) {
        const url = action === 'approve' ? 
            `/Admin/AdvanceRequests/Approve/${id}` : 
            `/Admin/AdvanceRequests/Reject/${id}`;
            
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
        .then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                alert('İşlem sırasında bir hata oluştu. Lütfen tekrar deneyin.');
            }
        })
        .catch(error => {
            console.error('Hata:', error);
            alert('İşlem sırasında bir hata oluştu. Lütfen tekrar deneyin.');
        });
    }
} 