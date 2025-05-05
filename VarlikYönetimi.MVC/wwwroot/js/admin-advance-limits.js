document.addEventListener('DOMContentLoaded', function() {
    // Form alanlarını başlangıçta devre dışı bırak
    const formInputs = document.querySelectorAll('input[type="number"]');
    const saveButton = document.querySelector('button[type="submit"]');
    const editButton = document.createElement('button');
    editButton.type = 'button';
    editButton.className = 'btn btn-primary me-2';
    editButton.innerHTML = '<i class="fas fa-edit me-1"></i> Düzenle';
    
    // Düzenle butonunu ekle
    saveButton.parentNode.insertBefore(editButton, saveButton);
    saveButton.style.display = 'none';

    // Düzenle butonuna tıklama olayı
    editButton.addEventListener('click', function() {
        formInputs.forEach(input => {
            input.disabled = false;
        });
        editButton.style.display = 'none';
        saveButton.style.display = 'inline-block';
    });

    // Form gönderildiğinde
    document.querySelector('form').addEventListener('submit', function(e) {
        e.preventDefault();
        
        const formData = {
            minAmount: parseFloat(formInputs[0].value),
            maxAmount: parseFloat(formInputs[1].value)
        };

        // JSON olarak kaydet
        localStorage.setItem('advanceLimits', JSON.stringify(formData));

        // Formu gönder
        this.submit();
    });

    // Sayfa yüklendiğinde kaydedilmiş değerleri yükle
    const savedLimits = localStorage.getItem('advanceLimits');
    if (savedLimits) {
        const limits = JSON.parse(savedLimits);
        formInputs[0].value = limits.minAmount;
        formInputs[1].value = limits.maxAmount;
    }

    // Tablo satırları için tıklama olayı
    const tableRows = document.querySelectorAll('tbody tr');
    tableRows.forEach(row => {
        row.addEventListener('click', function(e) {
            // Eğer tıklanan element buton değilse
            if (!e.target.closest('.btn-group')) {
                const editButton = this.querySelector('.btn-outline-primary');
                if (editButton) {
                    window.location.href = editButton.getAttribute('href');
                }
            }
        });
    });

    // Silme işlemi için onay
    const deleteButtons = document.querySelectorAll('.btn-outline-danger');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.stopPropagation(); // Tıklama olayının satıra yayılmasını engelle
            const id = this.getAttribute('data-id');
            if (confirm('Bu avans limitini silmek istediğinizden emin misiniz?')) {
                document.getElementById('deleteId').value = id;
                document.getElementById('deleteForm').submit();
            }
        });
    });
}); 