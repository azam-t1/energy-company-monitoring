import React, { useState } from 'react';

// API base URL
const API_BASE_URL = 'http://localhost:5233';

const MeterReadingUpload = () => {
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
      setError(null);
      setResult(null);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!file) {
      setError('Please select a CSV file');
      return;
    }

    if (file.type !== 'text/csv' && !file.name.endsWith('.csv')) {
      setError('Please select a valid CSV file');
      return;
    }
    
    setLoading(true);
    setError(null);
    
    const formData = new FormData();
    formData.append('file', file);
    
    try {
      const response = await fetch(`${API_BASE_URL}/meter-reading-uploads`, {
        method: 'POST',
        body: formData,
      });
      
      if (!response.ok) {
        throw new Error(`Server responded with status ${response.status}`);
      }
      
      const data = await response.json();
      setResult(data);
    } catch (err) {
      setError(`Error uploading file: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="meter-reading-upload">
      <h2>Meter Reading Upload</h2>
      
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="csvFile">Select Meter Reading CSV File:</label>
          <input 
            type="file" 
            id="csvFile" 
            accept=".csv" 
            onChange={handleFileChange} 
          />
        </div>
        
        <button type="submit" disabled={loading || !file}>
          {loading ? 'Uploading...' : 'Upload'}
        </button>
      </form>
      
      {error && <div className="error-message">{error}</div>}
      
      {result && (
        <div className="upload-result">
          <h3>Upload Results</h3>
          <div className="result-summary">
            <p><strong>Successful readings:</strong> {result.successfulReadings}</p>
            <p><strong>Failed readings:</strong> {result.failedReadings}</p>
          </div>
          
          {result.errors && result.errors.length > 0 && (
            <div className="error-details">
              <h4>Error Details:</h4>
              <ul className="error-list">
                {result.errors.map((error, index) => (
                  <li key={index}>{error}</li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default MeterReadingUpload;
