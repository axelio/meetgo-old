import React from 'react';
import ReactGA from 'react-ga';
import './ErrorBoundary.css';

class ErrorBoundary extends React.Component<any, { hasError: boolean }> {
    constructor(props: any) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error: any, info: any) {
        ReactGA.exception({
            description: `${error}. ${JSON.stringify(info)}`,
            fatal: true
        });
    }

    refreshPage = () => window.location.reload();

    render() {
        if (this.state.hasError) {
            return (
                <div className="error-wrapper">
                    <h1>Wystąpił niespodziewany błąd w aplikacji.</h1>
                    <h1 className='refresh-page' onClick={this.refreshPage}>Prosimy o odświeżenie strony.</h1>
                </div>
            );
        }

        return this.props.children;
    }
}
export default ErrorBoundary;