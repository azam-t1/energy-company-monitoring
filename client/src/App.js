import './App.css';
import MeterReadingUpload from './components/MeterReadingUpload';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Energy Company Monitoring</h1>
        <p>Upload and manage customer meter readings</p>
      </header>
      <main className="App-main">
        <MeterReadingUpload />
      </main>
    </div>
  );
}

export default App;
