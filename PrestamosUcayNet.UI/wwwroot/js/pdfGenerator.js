// Función para generar PDF desde HTML usando html2pdf.js
window.generarPDFDesdeHTML = async function (htmlContent, fileName) {
    try {
        // Verificar si html2pdf está disponible
        if (typeof html2pdf === 'undefined') {
            console.error('html2pdf.js no está cargado');
            alert('La librería de generación de PDF no está disponible. Por favor, recargue la página.');
            return;
        }

        // Configuración del PDF
        const opt = {
            margin: [10, 10, 10, 10],
            filename: fileName,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { 
                scale: 2,
                useCORS: true,
                letterRendering: true
            },
            jsPDF: { 
                unit: 'mm', 
                format: 'letter', 
                orientation: 'portrait' 
            },
            pagebreak: { mode: ['avoid-all', 'css', 'legacy'] }
        };

        // Generar y descargar el PDF
        await html2pdf().set(opt).from(htmlContent).save();

        console.log('PDF generado exitosamente:', fileName);
    } catch (error) {
        console.error('Error al generar PDF:', error);
        throw error;
    }
};
