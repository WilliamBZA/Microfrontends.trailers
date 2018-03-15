import * as React from 'react';
import { RouteComponentProps } from 'react-router';

import { TrailerView } from '../TrailerView';

export class Home extends React.Component<RouteComponentProps<{}>, any> {
    constructor() {
        super();

        this.state = {
            trailer: {
                loading: true
            }
        };

        fetch('api/Data/?moviedetails=Inxeba')
            .then(response => response.json())
            .then(data => {
                this.setState({
                    trailer: {
                        loading: false,
                        trailerId: data.trailerUrl
                    }
                });
            });
    }
    public render() {

        return <div>
            <TrailerView trailer={this.state.trailer} />
        </div>;
    }
}
