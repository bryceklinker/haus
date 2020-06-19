import React from "react";

import {Footer} from "./Footer";
import {MainContent} from "./MainContent";
import {Header} from "./Header";
import {Routes} from "../../shared/routing/routes";


export function Shell() {
    return (
        <div>
            <Header />
            
            <MainContent>
                <Routes />
            </MainContent>

            <Footer />    
        </div>
    )
}