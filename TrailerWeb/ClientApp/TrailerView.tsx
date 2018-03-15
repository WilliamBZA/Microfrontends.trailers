import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class TrailerView extends React.Component<any, any> {
    public render() {
        if (this.props.trailer.loading) {
            return <div><i>Loading...</i></div>;
        }

        return <div><iframe title="YouTube video player" className="youtube-player" type="text/html" width="640" height="390" src={"http://www.youtube.com/embed/" + this.props.trailer.trailerId } allowFullScreen></iframe>
        </div>;
    }
}