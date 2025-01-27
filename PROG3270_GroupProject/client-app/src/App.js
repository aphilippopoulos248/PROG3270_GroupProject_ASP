import React from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom"; // Import Link here
import logo from "./logo.svg";
import "./App.css";
import Home from "./Home"; // Make sure these components are created
import About from "./About";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <img src={logo} className="App-logo" alt="logo" />
                <p>Our Group Project</p>
                <p>Created by Alexander, Chanelle, and Kenan</p>
            </header>
            <main>
                <Router>
                    {/* Navigation Links */}
                    <nav>
                        <Link to="/" className="App-link">
                            Home
                        </Link>{" "}
                        |{" "}
                        <Link to="/about" className="App-link">
                            About
                        </Link>
                    </nav>
                    {/* Define Routes */}
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/about" element={<About />} />
                    </Routes>
                </Router>
            </main>
        </div>
    );
}

export default App;
